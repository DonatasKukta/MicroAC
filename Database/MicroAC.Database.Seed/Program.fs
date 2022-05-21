open MicroAC.Persistence.Entities

[<EntryPoint>]
let main argv =
    printfn "Let's generate data for performance tests!"
    printfn "Result folder is %s" (Config.getPath "")
    
    printfn "Generate random passwords:"
    Generate.passwords()

    Database.saveStateToScript("backup.sql")
    Database.removeSeededValues()
    
    let organisations =    Generate.organisations    
    let roles =            Generate.roles            
    let services =         Generate.services         
    let permissions =      Generate.permissions      services
    let rolesPermissions = Generate.rolesPermissions roles permissions
    let users =            Generate.users            organisations
    let usersRoles =       Generate.usersRoles       users roles

    Database.saveStateToScript("after_removing_seeded_data.sql")

    [
        Database.seed organisations    
        Database.seed roles            
        Database.seed services         
        Database.seed permissions      
        Database.seed rolesPermissions
        Database.seed users          
        Database.seed usersRoles     
    ] 
    |> Seq.sum 
    |> printfn "Total rows added: %i."

    Database.saveStateToScript("after_seeding_data.sql")
    
    users
    |> Seq.take 5 
    |> Seq.iter (fun u -> (Database.printUserRoles u.Id) )
    
    printfn "Seeding completed succesfully."
    0
