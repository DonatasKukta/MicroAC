module Config

open System
open System.IO
open FSharp.Data

[<Literal>]
let private settings = "appsettings.json"

type private configProvider = JsonProvider<settings>

let private config = configProvider.Load(settings)

let private date = DateTime.Now.ToString().Replace(':', '.')

let dbConnection = config.DbConnectionStr
let dbName = config.DbName
let exportDbUsername = config.DbServiceUsername
let getPath filename = Path.Combine(config.ResultFolder, date, filename) 
                                
Directory.CreateDirectory(getPath "") |> ignore


let squareRoot x = int (sqrt (float x))

let usersCount            = config.UserSeedCount
let rolesCount            = squareRoot usersCount
let permissionsCount      = rolesCount * 8  
let servicesCount         = squareRoot rolesCount   
let organisationsCount    = usersCount / 10
let rolesPermissionsCount = permissionsCount / rolesCount
