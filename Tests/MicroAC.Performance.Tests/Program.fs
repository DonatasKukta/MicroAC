open NBomber
open NBomber.FSharp
open FSharp.Control.Tasks
open Serilog

open System.Net.Http
open System.Net.Http.Json
open System.Threading

open Types
open NBomber.Configuration

let email i = $"testemail{i}@SeedTestData.com"

let users = 
    { 1 .. 1 .. 15000 } 
    |> Seq.map(fun i -> { Email= email i; Password= email i })
    |> Feed.createCircular "users"

let runTests() =
    Scenarios.GenerateScenarios()
    |> NBomberRunner.registerScenarios
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withReportFolder Config.reportsFolder
    |> NBomberRunner.withReportFormats [ReportFormat.Html; ReportFormat.Txt]
    |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.run
    |> ignore

let debug() =
    task {
        printf "Debug send operation"
        let http = new HttpClient()
        let req = new HttpRequestMessage(HttpMethod.Post, Config.loginUrl)
        let json = JsonContent.Create({ Email= email 1; Password= email 1 })
        req.Content <- json :> HttpContent
        let result = http.Send(req)
        let! response = StepDataHandling.readApiResponse<LoginResult> result "debugResponse" 
        response.timestamps 
        |> Seq.cast<string> 
        |> Seq.iter (fun x -> printfn "%A " x)
        Csv.appendTimestampsToCsv (new Mutex()) (Steps.toResult response) |> ignore
    }

let postTestCalculations() = 
    match System.IO.File.Exists(Config.timestampsCsv) with
    | false -> eprintfn "_timetsamps.csv file not found in reports folder. Ensure postScenarioHandling step es run at the end of at least one Scenario."
    | true -> 
        let timestamps = Csv.readTimestampsFromFile()
        let durations  = Csv.calcRequestDurations timestamps
        let averages   = Csv.calcRequestAverages durations
        Csv.writeDurationsToCsv       durations
        Csv.writeRequestAveragesToCsv averages
        Csv.calcAverageMatrixToCsv    averages

[<EntryPoint>]
let main argv =
    Csv.deleteCsvFiles()
    //debug() |> ignore
    runTests() |> ignore
    postTestCalculations()
    0
