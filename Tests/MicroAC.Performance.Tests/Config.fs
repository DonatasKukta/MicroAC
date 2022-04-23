module Config

open System
open System.IO
open System.Reflection
open FSharp.Data

open NBomber

[<Literal>]
let private settings = "appsettings.json"

type private configProvider = JsonProvider<settings>
let private config = configProvider.Load(settings)

let private date = DateTime.Now.ToString().Replace(':', '.')
let testLoadSize = config.TestLoadSize
let warmupEnabled = config.WarmupEnabled
let testDuration = minutes config.TestDurationMin
let getPath filename = Path.Combine(config.ReportsFolder, date, filename) 

let reportsFolder = getPath ""
let warmupReportsFolder = (getPath "") + "_warmup"
let timestampsCsv = getPath $"{date}_timestamps.csv"
let durationsCsv  = getPath $"{date}_durations.csv"
let averagesCsv   = getPath $"{date}_averages.csv"
let resultsCsv    = getPath $"{date}_results.csv" 

let coreConfigPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/appsettings.core.json"
let private coreConfig = JsonValue.Parse(File.ReadAllText coreConfigPath)

let centralAuthorizationEnabled = coreConfig.["CentralAuthorizationEnabled"].AsBoolean()
let clusterEndpoint = coreConfig.["SfClusterClientConnectionEndpoint"].AsString();
