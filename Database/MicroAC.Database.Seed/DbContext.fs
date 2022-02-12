module DbContext
open MicroAC.Persistence.DbDTOs
open FSharp.Data
open Microsoft.EntityFrameworkCore

[<Literal>]
let private settingsFile = "appsettings.persistance.json"
type private config = JsonProvider<settingsFile>

let get() = 
    let dbConnection = config.Load(settingsFile).ConnectionStrings.MicroAcDatabase
    let dbOptions = (new DbContextOptionsBuilder<MicroACContext>())
                        .UseSqlServer(dbConnection)
                        .Options
    new MicroACContext(dbOptions)