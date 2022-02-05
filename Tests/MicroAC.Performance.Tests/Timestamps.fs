module Timestamps

open System
open System.Globalization
open MicroAC.Core.Common
open Types

let parseDate (str : string)= 
    DateTime.ParseExact(
     str.Replace("-","/"), 
     Constants.TimestampFormat, 
     CultureInfo.InvariantCulture, 
     DateTimeStyles.None);

let fromSplitStr (split:seq<string>) =
    let date = split |> Seq.item 2 |> parseDate 
    {
        service =  split |> Seq.item 0; 
        action =  split |> Seq.item 1; 
        date = date;
        ms = date.Millisecond;
        msSum = 0;
        prevDiff = 0;
    }

let fromStrTimestamp (timestamp:string) = timestamp.Split("-") |> fromSplitStr

let getDurationDiff (future:DateTime) (past:DateTime) = 
    int (future - past).TotalMilliseconds

let calcTimestamp index current prev sum = 
    let msDiff = getDurationDiff current.date prev.date
    let msSum = sum + msDiff
    ((index,{current with msSum = msSum; prevDiff=msDiff}), msSum)

let mapWithPrev seq sum (it: int * Timestamp) = 
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

