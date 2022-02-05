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

let createLogin httpFactory url credentials = 
    Step.create("login",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->
             let postHandlingWithContext = postHandling<LoginResult> "loginResult" context
             Http.createRequest "POST" url
             |> Http.withHeader "Content-Type" "application/json"
             |> Http.withBody (JsonContent.Create credentials)
             |> Http.withCheck postHandlingWithContext 
             |> Http.send context
)

let createResourceAction httpFactory url = 
    Step.create("resourceApiAction",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let postHandlingWithContext = postHandling<string> "resourceApi" context
             let loginResponse = getApiResponse<LoginResult> "loginResult" context

             return! Http.createRequest "GET" url
                     |> Http.withHeader "Authorization" loginResponse.body.accessJwt
                     |> Http.withCheck postHandlingWithContext
                     |> Http.send context
        }
)

let createRefresh httpFactory url = 
    Step.create("refresh",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let postHandlingWithContext = postHandling<string> "refreshResult" context
             let loginResponse = getApiResponse<LoginResult> "loginResult" context 

             return! Http.createRequest "POST" url
                     |> Http.withBody (new StringContent(loginResponse.body.refreshJwt))
                     |> Http.withCheck postHandlingWithContext
                     |> Http.send context
        }
)

let postScenarioHandling = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        execute = fun context ->  task {
        let loginResponse =     getApiResponse<LoginResult> "loginResult" context 
        let refreshResponse =   getApiResponse<string> "refreshResult" context
        let resourceResponse =  getApiResponse<string> "resourceApi" context

        Csv.appendTimestampsToCsv loginResponse
        Csv.appendTimestampsToCsv refreshResponse
        Csv.appendTimestampsToCsv resourceResponse

        return Response.ok()
    })