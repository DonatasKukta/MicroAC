module StepDataHandling

open NBomber
open NBomber.Contracts
open NBomber.Plugins.Http.FSharp
open FSharp.Control.Tasks
open FSharp.Json
open System.Net.Http
open Types

let readContent<'content> (response : HttpResponseMessage) = 
    task {
        let! bodyStr = response.Content.ReadAsStringAsync()

        if(typeof<'content>.Equals(typeof<string>))
            then return unbox<'content> bodyStr
            else return Json.deserialize<'content> bodyStr
    }

let readApiResponse<'content> (response: HttpResponseMessage) = 
    task {
        let! body = readContent<'content> response
        let found, timestamps = response.Headers.TryGetValues "MicroAC-Timestamp"
        return { status = int response.StatusCode;  body = body; timestamps = timestamps;}
    }

let postHandling<'content> dataKey (stepContext : IStepContext<HttpClient, obj>) ( response: HttpResponseMessage)  = 
    task {
        let! apiResponse = readApiResponse<'content> response
        let added = stepContext.Data.TryAdd(dataKey, apiResponse)
        
        if(not added) then stepContext.StopCurrentTest $"PostHandling: Unable to add {dataKey} to context data."

        return Response.ofHttp(response)
    }

let getApiResponse<'content> key (stepContext : IStepContext<HttpClient, obj>) = 
    let found, value = stepContext.Data.TryGetValue key
    if (not found) then stepContext.StopCurrentTest $"Key {key} was not found in StepContext.Data"
    value :?> ApiResponse<'content>