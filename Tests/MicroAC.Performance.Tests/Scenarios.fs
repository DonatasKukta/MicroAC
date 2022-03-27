﻿module Scenarios

open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp

open System.Threading

open WebShop.Common
open Types

let data = new DataGenerator()
let email i = $"testemail{i}@SeedTestData.com"

let shipmentFeed = data.GenerateShipments()    |> Feed.createCircular "shipments" |> Some
let orderFeed    = data.GenerateOrders()       |> Feed.createCircular "orders"    |> Some
let cartItemFeed = [new CartItem()]            |> Feed.createCircular  "cartsItems"|> Some
let productFeed  = [new Product()]             |> Feed.createCircular  "products"  |> Some
let paymentFeed  = [new Order.PaymentDetails()]|> Feed.createCircular  "payments"  |> Some
let usersFeed = 
    { 1 .. 1 .. 15000} 
    |> Seq.map(fun i -> { Email= email i; Password= email i })
    |> Feed.createCircular "users"

let GenerateScenarios() =
    let httpFactory = HttpClientFactory.create()
    let csvMutex = new Mutex();
    
    let login    = Steps.login       httpFactory usersFeed
    let refresh  = Steps.refresh     httpFactory 
    let resource = Steps.resourceApi httpFactory 
 
    let getProduct    =  Steps.getProduct    httpFactory      
    let getProducts   =  Steps.getProducts   httpFactory      
    let createProduct =  Steps.createProduct httpFactory productFeed
    let updateProduct =  Steps.updateProduct httpFactory      
    let deleteProduct =  Steps.deleteProduct httpFactory      

    let getCart        = Steps.getCart        httpFactory     
    let createCart     = Steps.createCart     httpFactory     
    let addCartItem    = Steps.addCartItem    httpFactory cartItemFeed
    let deleteCart     = Steps.deleteCart     httpFactory     
    let deleteCartItem = Steps.deleteCartItem httpFactory     

    let getShipment    = Steps.getShipment    httpFactory     
    let getShipments   = Steps.getShipments   httpFactory     
    let createShipment = Steps.createShipment httpFactory shipmentFeed
    let updateShipment = Steps.updateShipment httpFactory shipmentFeed
    let deleteShipment = Steps.deleteShipment httpFactory     

    let getOrder       = Steps.getOrder       httpFactory     
    let getOrders      = Steps.getOrders      httpFactory     
    let createOrder    = Steps.createOrder    httpFactory orderFeed
    let deleteOrder    = Steps.deleteOrder    httpFactory     
    let submitShipment = Steps.submitShipment httpFactory shipmentFeed
    let submitPayment  = Steps.submitPayment  httpFactory paymentFeed
    let submitOrder    = Steps.submitOrder    httpFactory     

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
    
    let allSteps =  
        [
            login;
            refresh;
            resource;
            getProduct;
            getProducts;
            createProduct;
            updateProduct;
            deleteProduct;
            getCart;
            createCart;
            addCartItem;
            deleteCart;
            deleteCartItem;
            getShipment;
            getShipments;
            createShipment;
            updateShipment;
            deleteShipment;
            getOrder;
            getOrders;
            createOrder;
            deleteOrder;
            submitShipment;
            submitPayment;
            submitOrder;
            final;
        ] 
        |> Scenario.create "all steps" 
        |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
        |> Scenario.withoutWarmUp

    [
        allSteps;
        basicAuth; 
        dataAnalyst
    ]