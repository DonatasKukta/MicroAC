module Database

open MicroAC.Persistence.DbDTOs
open Microsoft.Data.SqlClient
open Microsoft.EntityFrameworkCore
open Microsoft.SqlServer.Management.Common
open Microsoft.SqlServer.Management.Smo

open System.Collections
open System.IO

// DBContext is initialised once
let private  context = 
    let dbOptions = (new DbContextOptionsBuilder<MicroACContext>())
                        .UseSqlServer(Config.dbConnection)
                        .Options
    new MicroACContext(dbOptions)

let getFirstUser() = 
    printfn "getFirstUser"
    context.Users |> Seq.head

let getLastUser() = 
    printfn "getLastUser"
    context.Users |> Seq.last
   
let backupScripts() = 
    let rec copy (enumerator: IEnumerator) (array: SqlSmoObject[]) (i:int) = 
        let onElement() = array.[i] <- enumerator.Current :?> SqlSmoObject
                          copy enumerator array (i+1)
        match enumerator.MoveNext() with 
            | false -> array
            | true -> onElement()
    let connection = new ServerConnection(new SqlConnection(Config.dbConnection))
    let sqlServer = new Server(connection)
    let database = sqlServer.Databases.Item Config.dbName
    let tables = database.Tables

    let scripter = new Scripter(sqlServer)
    scripter.Options.ScriptSchema <- true
    scripter.Options.ScriptData <- true
    scripter.Options.FileName <- Config.getPath "_backup.sql"
    scripter.Options.ToFileOnly <- true
    scripter.Options.NoCommandTerminator <- true
    scripter.Options.WithDependencies <- true
    
    let recArray = copy (tables.GetEnumerator()) (Array.zeroCreate<SqlSmoObject> tables.Count) 0 
    let result = scripter.EnumScript recArray

    File.WriteAllLines(Config.getPath "_backup-output.txt", result)    
    0