﻿module Config

open System
open System.IO
open System.Reflection
open FSharp.Data

[<Literal>]
let private settings = "appsettings.json"

type private configProvider = JsonProvider<settings>
let private config = configProvider.Load(settings)
 
let loginUrl = config.LoginUrl 
let refreshUrl = config.RefreshUrl
let resourceActionUrl = config.ResourceActionUrl
let webShopReverseProxyUrl = config.WebShopReverseProxyUrl

let private date = DateTime.Now.ToString().Replace(':', '.')
let getPath filename = Path.Combine(config.ReportsFolder, date, filename) 

let reportsFolder = getPath ""
let timestampsCsv = getPath "_timestamps.csv"
let durationsCsv  = getPath "_durations.csv"
let averagesCsv   = getPath "_averages.csv"
let matrixAvgCsv  = getPath "_matrixAvg.csv" 

let coreConfigPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/appsettings.core.json"
let private coreConfig = JsonValue.Parse(File.ReadAllText coreConfigPath)

let centralAuthorizationEnabled = coreConfig.["CentralAuthorizationEnabled"].AsBoolean()
