module Generate

open MicroAC.Persistence.DbDTOs


let users (count: int) = 
    raise (System.NotImplementedException())
    { 1 .. 1 .. count }
    |> Seq.iter (fun x -> printfn "%i" x)
    let user = new User(
                    Blocked = false
                    )
    0