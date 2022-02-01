module Timestamps

open System
open System.Globalization
open MicroAC.Core.Common
open Types

let fromStrTimestamp (timestamp:string) = 
    let split = timestamp.Split("-")
    let parsed, date = DateTime.TryParseExact(
                        split.[2], 
                        Constants.TimestampFormat, 
                        CultureInfo.InvariantCulture, 
                        DateTimeStyles.None);
    {
        service = split.[0]; 
        action = split.[1]; 
        date = date;
        ms = date.Millisecond;
        msSum = 0;
        prevDiff = 0;
    }

let calcTimestamp index current prev sum = 
    let msDiff = (current.date - prev.date).Milliseconds
    let msSum = sum + msDiff
    ((index,{current with msSum = msSum; prevDiff=msDiff}), msSum)

let mapWithPrev seq sum (it: int * Timestamp) = 
    printfn "mapWithPrev"
    let i, current = it
    let prev = Seq.tryItem (i - 1) seq
    match prev with
        | None -> ((0, current), 0)
        | Some prevu -> let p, prev = prevu
                        calcTimestamp i current prev sum

let ignoreIndex (i, x) = x
let ignoreAccumulator (x, a) = x
let mapTimestamps seq = Seq.mapFold (mapWithPrev seq) 0 seq 

let parseTimestamps timestamps = 
    timestamps
    |> Seq.cast<string>
    |> Seq.map fromStrTimestamp 
    |> Seq.indexed
    |> mapTimestamps
    |> ignoreAccumulator
    |> Seq.map ignoreIndex

