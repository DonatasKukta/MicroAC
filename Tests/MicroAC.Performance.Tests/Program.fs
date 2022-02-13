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

//TODO: Setup data feed.
let credentials  = { Email= "Jonas.Jonaitis@gmail.com"; Password= "" }
let users = 
    { 1 .. 1 .. 1000} 
    |> Seq.map(fun i -> { Email= $"testemail{i}@SeedTestData.com"; Password= "" })
    |> Feed.createCircular "users"

let runTests() =
    let httpFactory = HttpClientFactory.create()
    let csvMutex = new Mutex();

    let login =     Steps.createLogin          httpFactory users
    let resource =  Steps.createResource       httpFactory 
    let refresh =   Steps.createRefresh        httpFactory 
    let final =     Steps.postScenarioHandling csvMutex

    Scenario.create "debug" [login; resource; refresh; final]
    //|> Scenario.withLoadSimulations [KeepConstant(copies = 1, during = seconds 10)]
    |> Scenario.withLoadSimulations [InjectPerSec(rate = 60, during = minutes 5)]
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withReportFolder Config.reportsFolder
    //|> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.run
    |> ignore

let debug() =
    task {
        printf "Debug send operation"
        let http = new HttpClient()
        let req = new HttpRequestMessage(HttpMethod.Post, Config.loginUrl)
        let json = JsonContent.Create(credentials)
        req.Content <- json :> HttpContent
        let result = http.Send(req)
        let! response = StepDataHandling.readApiResponse<LoginResult> result "debugResponse" 
        response.timestamps 
        |> Seq.cast<string> 
        |> Seq.iter (fun x -> printfn "%A " x)
        Csv.appendTimestampsToCsv (new Mutex()) response |> ignore
    }

let postTestCalculations reportsFolder = 
    let timestamps =  Csv.readTimestampsFromFile reportsFolder
    let durations =   Csv.calcRequestDurations timestamps
    let averages =    Csv.calcRequestAverages durations
    Csv.writeDurationsToCsv       durations
    Csv.writeRequestAveragesToCsv averages
    Csv.calcAverageMatrixToCsv    averages

[<EntryPoint>]
let main argv =
    Csv.deleteCsvFiles()
    //debug() |> ignore
    //debug() |> ignore
    runTests() |> ignore
    postTestCalculations()
    0
