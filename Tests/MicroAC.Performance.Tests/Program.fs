open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open FSharp.Control.Tasks
open Serilog

open System
open System.IO
open System.Net.Http
open System.Net.Http.Json
open System.Threading

open Types

//TODO: Move to config
let loginUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login";
let refreshUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Refresh";
let resourceUrl = "http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/ResourceApi/Action";

//TODO: Setup data feed.
let credentials  = { Email= "Jonas.Jonaitis@gmail.com"; Password= "" }

let runTests reportsFolder =
    let httpFactory = HttpClientFactory.create()
    let csvMutex = new Mutex();

    let login =     Steps.createLogin           httpFactory loginUrl    credentials
    let resource =  Steps.createResourceAction  httpFactory resourceUrl 
    let refresh =   Steps.createRefresh         httpFactory refreshUrl 
    let final =     Steps.postScenarioHandling  reportsFolder csvMutex

    Scenario.create "debug" [login; resource; refresh; final]
    //|> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 60, during = seconds 10)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withReportFolder reportsFolder
    //|> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.run
    |> ignore

let debug reportsFolder =
    task {
        printf "Debug send operation"
        let http = new HttpClient()
        let req = new HttpRequestMessage(HttpMethod.Post, loginUrl)
        let json = JsonContent.Create(credentials)
        req.Content <- json :> HttpContent
        let result = http.Send(req)
        let! response = StepDataHandling.readApiResponse<LoginResult> result "debugResponse" 
        response.timestamps 
        |> Seq.cast<string> 
        |> Seq.iter (fun x -> printfn "%A " x)
        Csv.appendTimestampsToCsv reportsFolder (new Mutex()) response |> ignore
    }

let postTestCalculations reportsFolder = 
    let timestamps =  Csv.readTimestampsFromFile reportsFolder
    let durations =   Csv.calcRequestDurations timestamps
    let averages =    Csv.calcRequestAverages durations
    Csv.writeDurationsToCsv       reportsFolder durations
    Csv.writeRequestAveragesToCsv reportsFolder averages
    Csv.calcAverageMatrixToCsv    reportsFolder averages

[<EntryPoint>]
let main argv =
    let reportsFolder = Directory.GetCurrentDirectory() + $"/reports/{DateTime.Now.ToString().Replace(':', '.')}"
    Csv.deleteCsvFiles(reportsFolder)
    //debug() |> ignore
    //debug() |> ignore
    runTests(reportsFolder) |> ignore
    postTestCalculations(reportsFolder)
    0
