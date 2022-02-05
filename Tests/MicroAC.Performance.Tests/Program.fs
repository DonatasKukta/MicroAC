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
open System.Threading

//TODO: Move to config
let loginUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login";
let refreshUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Refresh";
let resourceUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/ResourceApi/Action";

//TODO: Setup data feed.
let credentials  = new LoginCredentials( Email= "Jonas.Jonaitis@gmail.com", Password= "")

let runTests () =
    let httpFactory = HttpClientFactory.create()
    let csvMutex = new Mutex();

    let login =     Steps.createLogin           httpFactory loginUrl    credentials
    let resource =  Steps.createResourceAction  httpFactory resourceUrl 
    let refresh =   Steps.createRefresh         httpFactory refreshUrl 
    let final =     Steps.postScenarioHandling  csvMutex

    Scenario.create "debug" [login; resource; refresh; final]
    //|> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 50, during = minutes 5)]
    |> Scenario.withoutWarmUp
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    //|> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Warning())
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
        Csv.appendTimestampsToCsv (new Mutex()) response |> ignore
    }

let postTestCalculations() = 
    let timestamps = Csv.readTimestampsFromFile()
    let durations = Csv.calcRequestDurations timestamps
    let averages = Csv.calcRequestAverages durations
    Csv.writeDurationsToCsv durations
    Csv.writeRequestAveragesToCsv averages

[<EntryPoint>]
let main argv =
    Csv.deleteCsvFiles()
    //debug() |> ignore
    runTests() |> ignore
    postTestCalculations()
    0
