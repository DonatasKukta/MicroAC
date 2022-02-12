[<EntryPoint>]
let main argv =
    printfn "Hello world!"
    let dbContext = DbContext.get()
    let firstUser = (dbContext.Users |> Seq.head)
    printfn "Email of first user is %A" firstUser.Email
    0