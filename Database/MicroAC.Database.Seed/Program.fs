open MicroAC.Persistence.DbDTOs

let printEmail (user: User) = printfn "User email: %A" (user.Email)

[<EntryPoint>]
let main argv =
    printfn "Hello world!"
    printfn "Working folder is %s" (Config.getPath "")

    Database.backupScripts()
    
    let organisations =     Generate.organisations    20
    let roles =             Generate.roles            10
    let services =          Generate.services         10
    let permissions =       Generate.permissions      50 services
    let rolesPermissions =  Generate.rolesPermissions 30 roles permissions
    let users =             Generate.users            20 organisations
    let usersRoles =        Generate.usersRoles       10 users roles
    0