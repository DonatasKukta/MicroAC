module StepDataHandling

open NBomber
open NBomber.Contracts
open NBomber.Plugins.Http.FSharp
open FSharp.Control.Tasks
open FSharp.Json
open System
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
        let foundt, timestamps = response.Headers.TryGetValues "MicroAC-Timestamp"
        let foundr, ids = response.Headers.TryGetValues "X-ServiceFabricRequestId" 
        let id = ids |> Seq.head |> Guid.Parse
        return { id = id; success = response.IsSuccessStatusCode;  body = body; timestamps = timestamps;}
    }

let postHandling<'content> dataKey (stepContext : IStepContext<HttpClient, obj>) ( response: HttpResponseMessage)  = 
    task {
        let! apiResponse = readApiResponse<'content> response
        let added = stepContext.Data.TryAdd(dataKey, apiResponse)
        
        match added with 
            | true -> return Response.ofHttp(response)
            | false -> return Response.fail($"Error.PostHandling: Unable to add {dataKey} to context data.", -2)
    }

let getApiResponse<'content> key (stepContext : IStepContext<HttpClient, obj>) = 
    let found, value = stepContext.Data.TryGetValue key
    match found with
        | true -> Some (value :?> ApiResponse<'content>)
        | false -> None