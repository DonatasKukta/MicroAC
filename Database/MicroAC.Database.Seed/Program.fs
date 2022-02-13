open MicroAC.Persistence.Entities

let printEmail (user: User) = printfn "User email: %A" (user.Email)

[<EntryPoint>]
let main argv =
    printfn "Hello world! This will generate data for performance tests!"
    printfn "Working folder is %s" (Config.getPath "")

    Database.saveStateToScript("backup.sql")
    
    let organisations =    Generate.organisations    40
    let roles =            Generate.roles            20
    let services =         Generate.services         10
    let permissions =      Generate.permissions      70   services
    let rolesPermissions = Generate.rolesPermissions roles permissions
    let users =            Generate.users            1000 organisations
    let usersRoles =       Generate.usersRoles       users roles
    
    // Clean database from previously seeded values.
    Database.removeSeededValues()
    Database.saveStateToScript("after_removing_seeded_data.sql")

    Database.seed organisations    
    Database.seed roles            
    Database.seed services         
    Database.seed permissions      
    Database.seed rolesPermissions
    Database.seed users          
    Database.seed usersRoles     

    Database.saveStateToScript("after_seeding_data.sql")
    
    users |> Seq.iter (fun u -> (Database.printUserRoles u.Id) )
    
    printfn "Seeding completed succesfully."
    0