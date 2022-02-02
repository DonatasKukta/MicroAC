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
    let split = str.Split(",")
    ( Guid.Parse(split.[0]), split |> Seq.skip 1 |> fromSplitStr)

let toTimestampCsvStr id t = $"{id},{t.service},{t.action},{formatDate t.date},{t.ms},{t.msSum},{t.prevDiff}"

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

//TODO: Fix error with incorrect date parsing
let readTimestampsFromFile() =
    File.ReadAllLines(csv)
    |> Seq.cast<string>
    |> Seq.map fromTimestampCsvStr
    |> Seq.groupBy (fun (guid, timestamp) -> guid)
