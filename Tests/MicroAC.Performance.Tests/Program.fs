open NBomber.FSharp
open NBomber.Configuration

open Serilog

open System

open Types

let email i = $"testemail{i}@SeedTestData.com"

let users = 
    { 1 .. 1 .. 30000 } 
    |> Seq.map(fun i -> { Email= email i; Password= email i })
    |> Feed.createCircular "users"

let runWarmup() = 
    let _, warmupScenario = Scenarios.GenerateScenarios()
    warmupScenario
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withReportFolder (Config.warmupReportsFolder())
    |> NBomberRunner.withReportFormats [ReportFormat.Html; ReportFormat.Txt]
    |> NBomberRunner.withLoggerConfig(fun () -> LoggerConfiguration().MinimumLevel.Verbose())
    |> NBomberRunner.withoutReports
    |> NBomberRunner.run
    |> ignore

let runTests() =
    let mainScenario, _ = Scenarios.GenerateScenarios()
    mainScenario
    |> NBomberRunner.registerScenario
    |> NBomberRunner.withTestSuite "http"
    |> NBomberRunner.withReportFolder (Config.reportsFolder())
    |> NBomberRunner.withReportFormats [ReportFormat.Html; ReportFormat.Txt]
    |> NBomberRunner.run
    |> ignore

let postTestCalculations() = 
    match System.IO.File.Exists(Config.timestampsCsv()) with
    | false -> eprintfn "_timetsamps.csv file not found in reports folder. Ensure postScenarioHandling step es run at the end of at least one Scenario."
    | true -> 
        let timestamps  = Csv.readTimestampsFromFile()
        let metrics     = Csv.calcMetrics timestamps
        let durations   = Csv.calcRequestDurations timestamps
        let averages    = Csv.calcRequestAverages durations
        Csv.appendMetricsToCsv metrics
        Csv.writeDurationsToCsv durations
        Csv.writeRequestAveragesToCsv averages
        Csv.appendCalcAverageMatrixToCsv averages
        Csv.appendServiceRequestCountsToCsv timestamps
        
let checkTestLoadSize (size: string option) = 
    match size with 
    | Some size -> Config.testLoadSize <- int size
    | None -> ()

[<EntryPoint>]
let main argv =    
    if Config.warmupEnabled then runWarmup()

    argv |> Seq.tryHead |> checkTestLoadSize |> ignore

    let started = DateTime.Now;
    runTests() |> ignore
    let completed = DateTime.Now;
    Csv.writeTestTimesToCsv started completed
    postTestCalculations()
    0
