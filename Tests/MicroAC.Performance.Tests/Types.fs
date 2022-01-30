module Types

open System.Collections


type LoginResult = { 
    accessJwt: string
    refreshJwt: string
}

type ApiResponse<'bodyType>(status: int, timestamps: IEnumerable, body:'bodyType) =
    member x.body = body
    member x.status = status
    member x.timestamps = timestamps

type LoginResponse = ApiResponse<LoginResult>

type RefreshResponse = ApiResponse<string>

type ResourceResponse = ApiResponse<string>