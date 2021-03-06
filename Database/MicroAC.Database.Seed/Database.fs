module Database

open Microsoft.Data.SqlClient
open Microsoft.SqlServer.Management.Common
open Microsoft.SqlServer.Management.Smo

open System
open System.Collections
open System.IO
open System.Linq

open MicroAC.Persistence
open MicroAC.Persistence.Entities
open Microsoft.EntityFrameworkCore

exception SeedingError of string

// DBContext is initialised once
let private  context = 
    let dbOptions = (new DbContextOptionsBuilder<MicroACContext>())
                        .UseSqlServer(Config.dbConnection)
                        .Options
    new MicroACContext(dbOptions)

let private seedValue = Generate.dataSeed

let private containsSeed<'T> (entity:'T) : bool = 
    match box entity with 
        | :? User            as entity -> entity.Name.Contains seedValue
        | :? RolesPermission as entity -> entity.Role.Contains seedValue
        | :? Service         as entity -> entity.Name.Contains seedValue
        | :? UsersRole       as entity -> entity.Role.Contains seedValue
        | :? Organisation    as entity -> entity.Name.Contains seedValue
        | :? Role            as entity -> entity.Name.Contains seedValue
        | :? Permission      as entity -> entity.Value.Contains seedValue
        | _ -> raise (SeedingError($"Entity type {entity.GetType().Name} not found during seed removal."))
    
let private removeSeed<'T when 'T: not struct> (set: DbSet<'T>)  = 
    let seededEntities = Seq.filter containsSeed set 
    set.RemoveRange(seededEntities)

let removeSeededValues() = 
    removeSeed context.Users 
    removeSeed context.Permissions
    removeSeed context.Roles
    removeSeed context.RolesPermissions
    removeSeed context.Services
    removeSeed context.UsersRoles
    removeSeed context.Organisations
    printfn "Deleted %i rows that had '%s' value in them." (context.SaveChanges()) seedValue
    
let seed<'T> (data: seq<'T>) = 
    match box data with 
        | :? seq<User>            as s -> context.Users.AddRange(s) 
        | :? seq<Permission>      as s -> context.Permissions.AddRange(s)
        | :? seq<Role>            as s -> context.Roles.AddRange(s)
        | :? seq<RolesPermission> as s -> context.RolesPermissions.AddRange(s)
        | :? seq<Service>         as s -> context.Services.AddRange(s)
        | :? seq<UsersRole>       as s -> context.UsersRoles.AddRange(s)
        | :? seq<Organisation>    as s -> context.Organisations.AddRange(s)
        | _ -> raise (SeedingError($"Data type {data.GetType().Name} not found during seeding."))
    let rowsCreated = context.SaveChanges()
    printfn "Created %i  rows of %A entity." rowsCreated typeof<'T> 
    rowsCreated
   
let saveStateToScript(filename) = 
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
    scripter.Options.FileName <- Config.getPath filename
    scripter.Options.ToFileOnly <- true
    scripter.Options.NoCommandTerminator <- true
    scripter.Options.WithDependencies <- true
    
    let recArray = copy (tables.GetEnumerator()) (Array.zeroCreate<SqlSmoObject> tables.Count) 0 
    let result = scripter.EnumScript recArray
    printfn "Database saved to file %s." filename
    File.WriteAllLines(Config.getPath "_backup-output.txt", result)  
    
let printUserRoles (guid: Guid) = 
    let user = context.Users.Find guid
    let uroles = 
        context.UsersRoles.ToList() 
        |> Seq.where (fun ur -> ur.User.Equals(guid)) 
        |> Seq.map (fun ur -> ur.Role)
    let permissions = 
        context.RolesPermissions.ToList() 
        |> Seq.where (fun rp -> Seq.contains rp.Role uroles)
        |> Seq.map (fun rp -> rp.Permission)
    
    let print list = Seq.iter (fun i -> printfn "   %A;" i) list
    let hex (bytes:byte[]) = bytes |> Array.fold (fun state x-> state + sprintf "%02X" x) ""
    printfn "================================"
    printfn "User-  guid:%A " guid 
    printfn "Email: %s Salt: %s Password: %s"user.Email (hex user.Salt) (hex user.PasswordHash)
    printfn "Roles:" 
    print uroles
    printfn "Permissions:" 
    print permissions
    printfn "================================"
