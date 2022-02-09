module Timestamps

open System
open System.Globalization
open Types

let timestampFormat = "yyyy/MM/ddThh:mm:ss.fffzzz"

let formatDate (date:DateTime) = date.ToString(timestampFormat)

let parseDate (str : string)= 
    DateTime.ParseExact(
     str.Replace("-","/"), 
     timestampFormat, 
     CultureInfo.InvariantCulture, 
     DateTimeStyles.None);

let fromSplitStr guid step (split:seq<string>) =
    let date = split |> Seq.item 2 |> parseDate 
    {
        id = guid;
        step = step;
        service =  split |> Seq.item 0;
        action =  split |> Seq.item 1; 
        date = date;
        ms = date.Millisecond;
        msSum = 0;
        prevDiff = 0;
    }

let fromStrTimestamp guid step (timestamp:string) = timestamp.Split("-") |> fromSplitStr guid step

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
let mapTimestampsWithPrev seq = Seq.mapFold (mapWithPrev seq) 0 seq 

let parseRawTimestamps guid step timestamps = 
    timestamps
    |> Seq.cast<string>
    |> Seq.map (fromStrTimestamp guid step)
    |> Seq.indexed
    |> mapTimestampsWithPrev
    |> ignoreAccumulator
    |> Seq.map ignoreIndex

