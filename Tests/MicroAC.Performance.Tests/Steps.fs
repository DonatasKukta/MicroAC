module Steps

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open System.Net.Http.Json
open FSharp.Control.Tasks
open System.Net.Http
open Types

let private globalTimeout = seconds 10

let private postHandling dataKey (stepContext : IStepContext<HttpClient, obj>) ( response: HttpResponseMessage)  = 
    task {
        let added = stepContext.Data.TryAdd(dataKey, response)
        
        if( not added) then stepContext.StopCurrentTest $"PostHandling: Unable to add {dataKey} to context data."
        
        if response.IsSuccessStatusCode 
            then 
                return Response.ok(response, int response.StatusCode)
            else 
                let! error = response.Content.ReadAsStringAsync()
                return Response.fail(error = error, statusCode = int response.StatusCode)
        }

let private getResponseValue key (stepContext : IStepContext<HttpClient, obj>) = 
    let found, value = stepContext.Data.TryGetValue key
    if (not found) then stepContext.StopCurrentTest $"Key {key} was not found in StepContext.Data"
    value :?> HttpResponseMessage

let createLogin httpFactory url credentials = 
    Step.create("login",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->
             let postHandlingWithContext = postHandling "loginResult" context
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
             let postHandlingWithContext = postHandling "resourceApi" context
             let loginResponse = getResponseValue "loginResult" context
             let! loginContent = loginResponse.Content.ReadFromJsonAsync(typedefof<LoginResult>) 
             let accessJwt = (loginContent :?> LoginResult).accessJwt

             return! Http.createRequest "GET" url
                     |> Http.withHeader "Authorization" accessJwt
                     |> Http.withCheck postHandlingWithContext
                     |> Http.send context
        }
)

let createRefresh httpFactory url = 
    Step.create("refresh",
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let postHandlingWithContext = postHandling "refreshResult" context
             let loginResponse = getResponseValue "loginResult" context 
             let! loginContent = loginResponse.Content.ReadFromJsonAsync(typedefof<LoginResult>) 
             let refreshJwt = (loginContent :?> LoginResult).refreshJwt

             return! Http.createRequest "POST" url
                     |> Http.withBody (new StringContent(refreshJwt))
                     |> Http.withCheck postHandlingWithContext
                     |> Http.send context
        }
)

let postScenarioHandling = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        execute = fun context ->  task {
        //let loginResponse = getResponseValue "loginResult" context 
        //let refreshResponse = getResponseValue "refreshResult" context
        //let resourceResponse = getResponseValue "resourceApi" context
        // TODO: Post-Scenario processing logic. Analyse headers, log data to files...
        return Response.ok()
    })