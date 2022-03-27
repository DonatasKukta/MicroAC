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
open WebShop.Common

let private globalTimeout = seconds 10
let private loginStep = "Login"
let private refreshStep = "Refresh"
let private resourceApiStep = "ResourceApi"

let toApiResponse<'a> (object:obj option) = (object.Value:?> ApiResponse<'a>)

// MicroAC Steps

let login httpFactory (users: IFeed<LoginInput>) = 
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

let resourceApi httpFactory = 
    Step.create(resourceApiStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string, obj> resourceApiStep context
             let loginResponseObj = getApiResponseNew loginStep context
             
             if(Option.isNone loginResponseObj) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let accessJwt = (loginResponseObj.Value:?> ApiResponse<LoginResult>).body.accessJwt
             return! Http.createRequest "GET" Config.resourceActionUrl
                     |> Http.withHeader "Authorization" accessJwt
                     |> Http.withCheck saveResponse
                     |> Http.send context
        }
)

let refresh httpFactory = 
    Step.create(refreshStep,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
             let saveResponse = saveResponseInContext<string, obj> refreshStep context
             let loginResponseObj = getApiResponseNew loginStep context 

             if(Option.isNone loginResponseObj) then return Response.fail("Refresh step couldn't get LoginResult", -1)
             else 
             let refreshJwt = (toApiResponse loginResponseObj).body.refreshJwt
             return!  Http.createRequest "POST" Config.refreshUrl
                         |> Http.withBody (new StringContent(refreshJwt))
                         |> Http.withCheck saveResponse
                         |> Http.send context
        }
)

// WebShop Steps

let withOptionalJsonBody body request = 
    match body with
    | null -> request 
    | body -> Http.withBody (JsonContent.Create body) request
    
let webshop (name:string) (service:Service) (action:Action) (httpFactory: IClientFactory<_>) (feed: IFeed<'a> option) = 
    Step.create(name,
        ?feed = feed,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
            let loginResponseObj = getApiResponseNew loginStep context 
            let accessJwt = 
                match Option.isNone loginResponseObj with
                | true -> ""
                | false -> (toApiResponse loginResponseObj).body.accessJwt
            
            let method = getWebShopHttpMethod action
            let url = getWebShopUrl service action
            let x = 0;
            return!  Http.createRequest method url
                        |> Http.withHeader "Authorization" accessJwt
                        |> withOptionalJsonBody context.FeedItem
                        |> Http.send context
        }
)

let getProducts (httpFactory: IClientFactory<_>) = 
    webshop "GetProducts" Service.Products Action.GetList httpFactory None

let getShipments (httpFactory: IClientFactory<HttpClient>) = 
    webshop "GetShipments" Service.Shipments Action.GetList httpFactory None

let getOrders (httpFactory: IClientFactory<HttpClient>) = 
    webshop "GetOrders" Service.Orders Action.GetList httpFactory None

let createProduct (httpFactory: IClientFactory<HttpClient>) (feed: IFeed<Product> option) = 
    webshop "CreateProduct" Service.Products Action.Create httpFactory feed
 
// Common Steps

let postScenarioHandling mutex = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        timeout = globalTimeout,
        execute = fun context ->  task {
        //TODO: Iterate context.Data and extract all api responses
        let login =     getApiResponseNew loginStep       context 
        let refresh =   getApiResponseNew refreshStep     context
        let resource =  getApiResponseNew resourceApiStep context

        if(Option.isSome login)     
            then Csv.appendTimestampsToCsv mutex (toApiResponse<LoginResult> login) |> ignore
        if(Option.isSome refresh) 
            then Csv.appendTimestampsToCsv mutex (toApiResponse<string> refresh) |> ignore
        if(Option.isSome resource) 
            then Csv.appendTimestampsToCsv mutex (toApiResponse<string> resource) |> ignore

        return Response.ok()
    })