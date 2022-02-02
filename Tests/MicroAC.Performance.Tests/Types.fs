﻿module Types

open System.Collections
open System


type LoginResult = { 
    accessJwt: string
    refreshJwt: string
}

type ApiResponse<'bodyType> = {
    id: Guid;
    success: bool;
    timestamps: IEnumerable;
    body:'bodyType;
}
    
type Timestamp = {
    service : string;
    action : string;
    date : DateTime;
    ms: int;
    msSum : int;
    prevDiff: int;
}
    