module Scenarios

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp

open System.Threading

open WebShop.Common
open Types

// WebShop Data Feeds:

let data = new DataGenerator()
let email i = $"testemail{i}@SeedTestData.com"

let shipmentsFeed = data.GenerateShipments() |>  Feed.createCircular "shipments"
let ordersFeed = data.GenerateOrders() |>  Feed.createCircular "orders"
let cartsFeed = [new WebShopCart()]  |> Feed.createCircular "carts"
let productsFeed = [new Product()]  |> Feed.createCircular "products"
let usersFeed = 
    { 1 .. 1 .. 15000} 
    |> Seq.map(fun i -> { Email= email i; Password= email i })
    |> Feed.createCircular "users"

let GenerateScenarios() =
    let httpFactory = HttpClientFactory.create()
    let csvMutex = new Mutex();
    
    let login =    Steps.login       httpFactory usersFeed
    let refresh =  Steps.refresh     httpFactory 
    let resource = Steps.resourceApi httpFactory 
    
    let getProducts   = Steps.getProducts   httpFactory
    let getShipments  = Steps.getShipments  httpFactory
    let getOrders     = Steps.getOrders    httpFactory
    let createProduct = Steps.createProduct httpFactory (Some productsFeed)
    
    let final = Steps.postScenarioHandling csvMutex
    
    let basicAuth = 
        Scenario.create "basic auth" [login; refresh; resource; final]
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
        //|> Scenario.withLoadSimulations [InjectPerSec(rate = 60, during = minutes 5)]
    
    let dataAnalyst =  
        Scenario.create "data analyst" [login; getProducts; getShipments; getOrders; createProduct;] 
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
        |> Scenario.withoutWarmUp
        //|> Scenario.withLoadSimulations [InjectPerSec(rate = 60, during = minutes 5)]
    
    [
        basicAuth; 
        dataAnalyst
    ]
