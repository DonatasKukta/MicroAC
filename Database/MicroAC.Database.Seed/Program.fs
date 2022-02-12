open MicroAC.Persistence.DbDTOs

let printEmail (user: User) = printfn "User email: %A" (user.Email)

[<EntryPoint>]
let main argv =
    printfn "Hello world!"
    printfn "Working folder is %s" (Config.getPath "")

    printEmail (Database.getFirstUser())
    printEmail (Database.getLastUser())

    Database.backupScripts() |> ignore
    //Generate.users 20
    0