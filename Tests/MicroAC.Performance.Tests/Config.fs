module Config

open System
open System.IO
open System.Reflection
open FSharp.Data

open NBomber

let coreConfigPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/appsettings.core.json"
let private coreConfig = JsonValue.Parse(File.ReadAllText coreConfigPath)

let centralAuthEnabled = coreConfig.["CentralAuthorizationEnabled"].AsBoolean()
let clusterEndpoint = coreConfig.["SfClusterClientConnectionEndpoint"].AsString();

[<Literal>]
let private settings = "appsettings.json"

type private configProvider = JsonProvider<settings>
let private config = configProvider.Load(settings)

let private date = DateTime.Now.ToString().Replace(':', '.')
let centralStr = match centralAuthEnabled with | true -> "Central" | false -> "NotCentral"

let mutable testLoadSize = config.TestLoadSize
let warmupEnabled = config.WarmupEnabled
let testDuration = minutes config.TestDurationMin

let private subFolder() = $"{testLoadSize}_{centralStr}_{date}" 
let getPath filename = Path.Combine(config.ReportsFolder, subFolder(), filename) 

let reportsFolder() = getPath ""
let warmupReportsFolder() = (getPath "") + "_warmup"
let timestampsCsv() = getPath $"{subFolder()}_timestamps.csv"
let durationsCsv()  = getPath $"{subFolder()}_durations.csv"
let averagesCsv()   = getPath $"{subFolder()}_averages.csv"
let resultsCsv()    = getPath $"{subFolder()}_results.csv" 
