﻿module StepDataHandling

open NBomber
open NBomber.Contracts
open NBomber.Plugins.Http.FSharp
open FSharp.Control.Tasks
open FSharp.Json
open System
open System.Net.Http
open Types
open MicroAC.Core.Constants

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
        let id = ids |> Seq.head |> Guid.Parse
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
    | Action.AddCartItem   
    | Action.Create -> "POST"

let getWebShopUrl service action = 
    let servicePath = 
        match service with
        | Service.Cart      -> "Carts/"
        | Service.Products  -> "Products/"
        | Service.Orders    -> "Orders/"
        | Service.Shipments -> "Shipments/"

    let id() = Guid.NewGuid().ToString()

    let path = 
        match (service, action) with 
        // Special Cases:
        | (Service.Cart,    Action.AddCartItem)    -> $"{id()}/products"
        | (Service.Cart,    Action.DeleteCartItem) -> $"{id()}/products/{id()}"
        | (Service.Orders,  Action.SubmitPayment)  -> $"{id()}/payment"
        | (Service.Orders,  Action.SubmitShipment) -> $"{id()}/shipment"
        | (Service.Orders,  Action.Create)
        | (Service.Shipments, _)
        // Common cases
        | (_, Action.Update) 
        | (_, Action.Delete) 
        | (_, Action.GetOne) -> id()
        | (_, Action.GetList) 
        | (_, Action.Create) -> ""
        | (service , action) -> $"Unrecognised {service} and {action} path combination."

    Config.webShopBaseUrl + servicePath + path