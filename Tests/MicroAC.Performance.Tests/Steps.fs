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


let createLogin httpFactory (users: IFeed<LoginInput>) = 
    Step.create(loginStep,
        feed = users,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->
             let saveResponse = saveResponseInContext<LoginResult, LoginInput> loginStep context
             Http.createRequest "POST" Config.loginUrl
             |> Http.withHeader "Content-Type" "application/json"
             |> Http.withBody (JsonContent.Create context.FeedItem)
             |> Http.withCheck saveResponse 
             |> Http.send context
)

let createResource httpFactory = 
    Step.create(resourceApiStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string, obj> resourceApiStep context
             let loginResponse = getApiResponse<LoginResult, obj> loginStep context
             
             if(Option.isNone loginResponse) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let accessJwt = loginResponse.Value.body.accessJwt
             return! Http.createRequest "GET" Config.resourceActionUrl
                     |> Http.withHeader "Authorization" accessJwt
                     |> Http.withCheck saveResponse
                     |> Http.send context
        }
)

let createRefresh httpFactory = 
    Step.create(refreshStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string, obj> refreshStep context
             let loginResponse = getApiResponse<LoginResult, obj> loginStep context 

             if(Option.isNone loginResponse) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let refreshJwt = loginResponse.Value.body.refreshJwt
             return!  Http.createRequest "POST" Config.refreshUrl
                         |> Http.withBody (new StringContent(refreshJwt))
                         |> Http.withCheck saveResponse
                         |> Http.send context
        }
)

let postScenarioHandling mutex = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        execute = fun context ->  task {
        let login =     getApiResponse<LoginResult, obj> loginStep       context 
        let refresh =   getApiResponse<string, obj>      refreshStep     context
        let resource =  getApiResponse<string, obj>      resourceApiStep context

        if(Option.isSome login)     
            then Csv.appendTimestampsToCsv mutex login.Value |> ignore
        if(Option.isSome refresh) 
            then Csv.appendTimestampsToCsv mutex refresh.Value |> ignore
        if(Option.isSome resource) 
            then Csv.appendTimestampsToCsv mutex resource.Value |> ignore

        return Response.ok()
    })