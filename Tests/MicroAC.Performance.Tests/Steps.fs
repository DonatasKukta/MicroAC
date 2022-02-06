module Steps

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open System.Net.Http.Json
open FSharp.Control.Tasks
open System.Net.Http
open Types
open StepDataHandling

let private globalTimeout = seconds 10
let private loginStep = "Login"
let private refreshStep = "Refresh"
let private resourceApiStep = "ResourceApi"


let createLogin httpFactory url credentials = 
    Step.create("login",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->
             let saveResponse = saveResponseInContext<LoginResult> loginStep context
             Http.createRequest "POST" url
             |> Http.withHeader "Content-Type" "application/json"
             |> Http.withBody (JsonContent.Create credentials)
             |> Http.withCheck saveResponse 
             |> Http.send context
)

let createResourceAction httpFactory url = 
    Step.create(resourceApiStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string> resourceApiStep context
             let loginResponse = getApiResponse<LoginResult> loginStep context
             
             if(Option.isNone loginResponse) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let accessJwt = loginResponse.Value.body.accessJwt
             return! Http.createRequest "GET" url
                     |> Http.withHeader "Authorization" accessJwt
                     |> Http.withCheck saveResponse
                     |> Http.send context
        }
)

let createRefresh httpFactory url = 
    Step.create(refreshStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string> refreshStep context
             let loginResponse = getApiResponse<LoginResult> loginStep context 

             if(Option.isNone loginResponse) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let refreshJwt = loginResponse.Value.body.refreshJwt
             return!  Http.createRequest "POST" url
                         |> Http.withBody (new StringContent(refreshJwt))
                         |> Http.withCheck saveResponse
                         |> Http.send context
        }
)

let postScenarioHandling mutex = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        execute = fun context ->  task {
        let login =     getApiResponse<LoginResult> loginStep context 
        let refresh =   getApiResponse<string> "Refresh" context
        let resource =  getApiResponse<string> resourceApiStep context

        if(Option.isSome login)     
            then Csv.appendTimestampsToCsv mutex login.Value |> ignore
        if(Option.isSome refresh) 
            then Csv.appendTimestampsToCsv mutex refresh.Value |> ignore
        if(Option.isSome resource) 
            then Csv.appendTimestampsToCsv mutex resource.Value |> ignore

        return Response.ok()
    })