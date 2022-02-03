module Csv

open Types
open Timestamps
open System.IO
open System
open MicroAC.Core.Common

type TimestampCsv = (Guid * Timestamp)

let csv = "timestamps.csv"

let deleteCsv() = File.Delete csv

let formatDate (date:DateTime) = date.ToString(Constants.TimestampFormat)

let fromTimestampCsvStr (str : string) = 
    let split = str.Split(";")
    ( Guid.Parse(split.[0]), split |> Seq.skip 1 |> fromSplitStr)

let toTimestampCsvStr id t = $"{id};{t.service};{t.action};{formatDate t.date};{t.ms};{t.msSum};{t.prevDiff}"

/// <summary>
/// Appends timestamps to csv file.
/// Ignores timestamps where response.success is false.
/// </summary>
let appendTimestampsToCsv (response:ApiResponse<'bodyType> ) = 
    if (not response.success) then ()
    File.AppendAllLines(csv , response.timestamps 
                              |> parseTimestamps 
                              |> Seq.map (toTimestampCsvStr response.id))
    |> ignore

let find (timestamps: seq<Timestamp>) action service = 
    timestamps |> Seq.find (fun t -> t.action = action && t.service = service) 

let getServiceDurations (timestamps:seq<Timestamp>) = 
    let findEndOf = find timestamps "End"
    timestamps 
    |> Seq.filter (fun t -> t.action = "Start")
    |> Seq.map (fun(s)-> let e = findEndOf s.service
                         let duration = (e.date - s.date).Milliseconds
                         (s.service, duration) )

let mapResult (guid, durations) = 
    Seq.map (fun sd -> let service, duration = sd
                       $"{guid};{service};{duration}") durations

let readTimestampsFromFile() =
    File.ReadAllLines(csv)
    |> Seq.cast<string>
    |> Seq.map fromTimestampCsvStr
    
let calcRequestDurations guidTimestamps =
    guidTimestamps
    |> Seq.groupBy (fun (guid, timestamp) -> guid)
    |> Seq.map (fun (guid, gt) -> ( (guid, getServiceDurations (Seq.map (fun(g,t)-> t) gt))))
    |> Seq.map mapResult
    |> Seq.reduce Seq.append
    