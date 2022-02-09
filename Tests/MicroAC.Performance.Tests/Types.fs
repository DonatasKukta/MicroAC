﻿module Types

open System.Collections
open System

type LoginResult = { 
    accessJwt: string
    refreshJwt: string
}

type LoginInput = {
    Email: string;
    Password: string;
}

type ApiResponse<'bodyType> = {
    id: Guid;
    step: string;
    success: bool;
    timestamps: IEnumerable;
    body:'bodyType;
}
    
type Timestamp = {
    id: Guid;
    step:string;
    service: string;
    action: string;
    date: DateTime;
    ms: int;
    msSum: int;
    prevDiff: int;
}

type Duration = {
    requestId: Guid;
    step: string;
    service: string;
    duration: int
}

type Average = {
    step: string;
    service: string;
    average: double;
}