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
open MicroAC.Core.Constants

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
                     |> Http.withHeader HttpHeaders.Authorization accessJwt
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
    
let webshop name service action httpFactory feed = 
    Step.create(name,
        ?feed = feed,
        clientFactory = httpFactory,
        timeout = globalTimeout,
        execute = fun context ->  task {
            let saveResponse = saveResponseInContext<string, 'a> name context

            let loginResponseObj = getApiResponseNew loginStep context 
            let accessJwt = 
                match Option.isNone loginResponseObj with
                | true -> ""
                | false -> (toApiResponse loginResponseObj).body.accessJwt
            
            let method = getWebShopHttpMethod action
            let url = getWebShopUrl service action

            return!  Http.createRequest method url
                        |> Http.withHeader HttpHeaders.Authorization accessJwt
                        |> withOptionalJsonBody context.FeedItem
                        |> Http.withCheck saveResponse
                        |> Http.send context
        }
)

let getProduct     factory       = webshop "GetProduct"     Service.Products Action.GetOne  factory None
let getProducts    factory       = webshop "GetProducts"    Service.Products Action.GetList factory None
let createProduct  factory feed  = webshop "CreateProducts" Service.Products Action.Create  factory feed
let updateProduct  factory       = webshop "UpdateProducts" Service.Products Action.Update  factory None
let deleteProduct  factory       = webshop "DeleteProducts" Service.Products Action.Delete  factory None

let getCart        factory      = webshop "GetCart"        Service.Cart Action.GetOne         factory None
let createCart     factory      = webshop "CreateCart"     Service.Cart Action.Create         factory None
let addCartItem    factory feed = webshop "AddCartItem"    Service.Cart Action.AddCartItem    factory feed
let deleteCart     factory      = webshop "DeleteCart"     Service.Cart Action.Delete         factory None
let deleteCartItem factory      = webshop "DeleteCartItem" Service.Cart Action.DeleteCartItem factory None

let getShipment    factory      = webshop "GetShipment"     Service.Shipments Action.GetOne  factory None
let getShipments   factory      = webshop "GetShipments"    Service.Shipments Action.GetList factory None
let createShipment factory feed = webshop "CreateShipments" Service.Shipments Action.Create  factory feed
let updateShipment factory feed = webshop "UpdateShipments" Service.Shipments Action.Update  factory feed
let deleteShipment factory      = webshop "DeleteShipments" Service.Shipments Action.Delete  factory None

let getOrder       factory      = webshop "GetOrder"            Service.Orders Action.GetOne         factory None
let getOrders      factory      = webshop "GetOrders"           Service.Orders Action.GetList        factory None
let createOrder    factory      = webshop "CreateOrder"         Service.Orders Action.Create         factory None
let deleteOrder    factory      = webshop "DeleteOrders"        Service.Orders Action.Delete         factory None
let submitShipment factory feed = webshop "SubmitOrderShipment" Service.Orders Action.SubmitShipment factory feed
let submitPayment  factory feed = webshop "SubmitOrderPayment"  Service.Orders Action.SubmitPayment  factory feed
let submitOrder    factory      = webshop "SubmitOrder"         Service.Orders Action.Delete         factory None

// Common Steps

let toResult (response : ApiResponse<'t>) = 
    { 
        id = response.id; 
        success = response.success;  
        timestamps = response.timestamps;
        step = response.step;
    }

let postScenarioHandling mutex = 
    Step.create("post_scenario_handling", 
        doNotTrack = true,
        timeout = globalTimeout,
        execute = fun context ->  task {

        let write = Csv.appendTimestampsToCsv mutex 

        context.Data
        |> Seq.filter (fun d ->  not (d.Key.Equals "nbomber_step_response"))
        |> Seq.map (fun d -> d.Value)
        |> Seq.map (fun r -> match r with 
                              | :? ApiResponse<LoginResult> as x -> toResult x
                              | :? ApiResponse<string> as y -> toResult y
                              | _ -> raise (System.Exception("Unrecognised body type"))
                              )
        |> Seq.iter (fun r -> (write r) |> ignore)

        return Response.ok()
    })