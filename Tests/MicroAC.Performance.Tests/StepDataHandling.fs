module StepDataHandling

open NBomber
open NBomber.Contracts
open NBomber.Plugins.Http.FSharp
open FSharp.Control.Tasks
open FSharp.Json
open System
open System.Net.Http
open Types
open MicroAC.Core.Constants
open MicroAC.Core.Client

let readContent<'content> (response : HttpResponseMessage) = 
    task {
        let! bodyStr = response.Content.ReadAsStringAsync()

        if(typeof<'content>.Equals(typeof<string>))
            then return unbox<'content> bodyStr
            else return Json.deserialize<'content> bodyStr
    }

let readApiResponse<'content> (response: HttpResponseMessage) step = 
    task {
        let! body = readContent<'content> response
        let foundt, timestamps = response.Headers.TryGetValues HttpHeaders.Timestamps
        let foundr, ids = response.Headers.TryGetValues "X-ServiceFabricRequestId" 
        let id = if foundr then ids |> Seq.head |> Guid.Parse else Guid.NewGuid()
        return { 
            id = id; 
            success = response.IsSuccessStatusCode;  
            body = body; 
            timestamps = timestamps;
            step = step;
            }
    }

let saveResponseInContext<'content,'a> stepKey (stepContext : IStepContext<HttpClient, 'a>) ( response: HttpResponseMessage)  = 
    task {
        let! apiResponse = readApiResponse<'content> response stepKey
        let added = stepContext.Data.TryAdd(stepKey, apiResponse)
        
        match added with 
            | true -> return Response.ofHttp(response)
            | false -> return Response.fail($"Error.PostHandling: Unable to add {stepKey} to context data.", -2)
    }

let getApiResponseNew stepKey (stepContext : IStepContext<_,_>) = 
    let found, value = stepContext.Data.TryGetValue stepKey
    match found with
        | true -> Some value
        | false -> None

let getWebShopHttpMethod action = 
    match action with
    | Action.SubmitPayment 
    | Action.SubmitShipment
    | Action.Update -> "PUT"
    | Action.DeleteCartItem
    | Action.Delete -> "DELETE"
    | Action.GetOne 
    | Action.GetList -> "GET"
    | Action.Login
    | Action.Refresh
    | Action.AddCartItem   
    | Action.Create -> "POST"

let endpointResolver = new FabricEndpointResolver();
endpointResolver.InitialiseEndpoints();


let getWebShopUrl service action = 
    let baseUrl = 
        match Config.centralAuthorizationEnabled with 
        | true -> $"{endpointResolver.GetServiceEndpoint(Service.RequestManager)}/{Enum.GetName service}"
        | false -> endpointResolver.GetServiceEndpoint(service)
    
    let id = Guid.NewGuid().ToString()

    let path = 
        match (service, action) with 
        // Special Cases:
        | (Service.Cart,    Action.AddCartItem)    -> $"/{id}/products"
        | (Service.Cart,    Action.DeleteCartItem) -> $"/{id}/products/{id}"
        | (Service.Orders,  Action.SubmitPayment)  -> $"/{id}/payment"
        | (Service.Orders,  Action.SubmitShipment) -> $"/{id}/shipment"
        | (Service.Orders,  Action.Create)
        | (Service.Shipments, _)
        // Common cases
        | (_, Action.Update) 
        | (_, Action.Delete) 
        | (_, Action.GetOne) -> $"/{id}"
        | (_, Action.GetList) 
        | (_, Action.Create) -> ""
        | (Service.Authentication, Action.Login)   -> "/Login"
        | (Service.Authentication, Action.Refresh) -> "/Refresh"
        | (service , action) -> $"Unrecognised {service} and {action} path combination."

    baseUrl + path