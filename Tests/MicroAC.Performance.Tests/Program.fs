open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open System.Net.Http.Json
open MicroAC.Core.Models;
open FSharp.Control.Tasks
open System.Net.Http
open Serilog
open Types

//TODO: Move to config
let loginUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login";
let refreshUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Refresh";
let resourceUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/ResourceApi/Action";

//TODO: Setup data feed.
let credentials  = new LoginCredentials( Email= "Jonas.Jonaitis@gmail.com", Password= "")

let runTests () =
    let httpFactory = HttpClientFactory.create()

    let login =     Steps.createLogin           httpFactory loginUrl    credentials
    let resource =  Steps.createResourceAction  httpFactory resourceUrl 
    let refresh =   Steps.createRefresh         httpFactory refreshUrl 
    let final =     Steps.postScenarioHandling

    Scenario.create "debug" [login; resource; refresh; final]
    |> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    //|> Scenario.withLoadSimulations [InjectPerSec(rate = 60, during = minutes 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.run
    |> ignore

let debug () =
    task {
        printf "Debug send operation"
        let http = new HttpClient()
        let req = new HttpRequestMessage(HttpMethod.Post, loginUrl)
        let json = JsonContent.Create(credentials)
        req.Content <- json :> HttpContent
        let result = http.Send(req)
        let! response = StepDataHandling.readApiResponse<LoginResult> result 
        response.timestamps 
        |> Seq.cast<string> 
        |> Seq.iter (fun x -> printfn "%A " x)
        Csv.appendTimestampsToCsv response
    }

[<EntryPoint>]
let main argv =
    Csv.deleteCsv()
    debug() |> ignore
    debug() |> ignore
    let fromFile = Csv.readTimestampsFromFile()
    //runTests() |> ignore
    0
