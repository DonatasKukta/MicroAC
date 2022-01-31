module Timestamps

open System
open System.Collections
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

let updateMsSum current prev = 
    let diff =  current.date - prev.date 
    let newSum = prev.msSum + diff.Milliseconds
    {current with msSum = newSum}
    
let updatePrevDiff current prev = 
    let diff =  current.date - prev.date 
    {current with prevDiff = diff.Milliseconds}

let mapWithPrev seq func (it: int * Timestamp) = 
    let i, current = it
    let prev = Seq.tryItem (i - 1) seq
    match prev with
        | None -> (0, current)
        | Some prevu ->
            let p, prev = prevu
            (i, func current prev)

let removeIndex (i, x) = x
let ignoreAcumulator (seq, acc) = seq
let seqMapSumWithPrev seq = Seq.map (mapWithPrev seq updateMsSum) seq 
let seqMapDiffWithPrev seq = Seq.map (mapWithPrev seq updatePrevDiff) seq 

let parseTimestamps timestamps = 
    timestamps
    |> Seq.cast<string>
    |> Seq.map fromStrTimestamp 
    |> Seq.indexed
    |> seqMapSumWithPrev
    //TODO: Understand why seqMapSumWithPrev and seqMapDiffWithPrev won't work together
    //|> seqMapDiffWithPrev
    |> Seq.map removeIndex

