module Csv

open Types
open Timestamps
open System.IO
open System
open MicroAC.Core.Common
open System.Threading

type TimestampCsv = (Guid * Timestamp)

let timestampsCsv = "_timestamps.csv"
let durationsCsv  = "_durations.csv"
let averagesCsv   = "_averages.csv"

let deleteFile file =
    if (File.Exists file) then File.Delete file

let deleteCsvFiles() = 
    deleteFile timestampsCsv
    deleteFile durationsCsv
    deleteFile averagesCsv

let formatDate (date:DateTime) = date.ToString(Constants.TimestampFormat)

let fromTimestampCsvStr (str : string) = 
    let split = str.Split(";")
    fromSplitStr (Guid.Parse(split.[0])) split.[1] (Seq.skip 2 split)

let toTimestampCsvStr t = $"{t.id};{t.step};{t.service};{t.action};{formatDate t.date};{t.ms};{t.msSum};{t.prevDiff}"

/// <summary>
/// Appends timestamps to csv file.
/// Ignores timestamps where response.success is false.
/// </summary>
let appendTimestampsToCsv (mutex : Mutex) (response:ApiResponse<'bodyType> ) = 
    if (not response.success) then ()
    let timestampsStr = response.timestamps 
                        |> (parseRawTimestamps response.id response.step)
                        |> Seq.map toTimestampCsvStr
    mutex.WaitOne() |> ignore
    File.AppendAllLines(timestampsCsv , timestampsStr)
    mutex.ReleaseMutex() 
    timestampsStr

let writeDurationsToCsv (durations: seq<Guid * string * int >) = 
    let str = durations |> Seq.map (fun (g, s, d) -> $"{g};{s};{d};")
    File.WriteAllLines(durationsCsv, str)
    
let writeRequestAveragesToCsv (averages: seq<string * double >) = 
    let str = averages |> Seq.map (fun (s, a) -> $"{s};{a};")
    File.WriteAllLines(averagesCsv, str)

let find (timestamps: seq<Timestamp>) action service = 
    timestamps |> Seq.find (fun t -> t.action = action && t.service = service) 

let readTimestampsFromFile() =
    File.ReadAllLines(timestampsCsv)
    |> Seq.cast<string>
    |> Seq.map fromTimestampCsvStr

let getServiceDurations (timestamps:seq<Timestamp>) = 
    let findEndOf = find timestamps "End"
    timestamps 
    |> Seq.filter (fun t -> t.action = "Start")
    |> Seq.map (fun(s)-> let e = findEndOf s.service
                         let duration = getDurationDiff e.date s.date
                         (s.id, s.service, duration) )

let calcRequestDurations timestamps =
    timestamps
    |> Seq.groupBy (fun (timestamp) -> timestamp.id)
    |> Seq.map (fun (guid, t) -> ( (getServiceDurations t(*(Seq.map (fun(g,t)-> t) gt)*))))
    |> Seq.concat
    
let calcMsAverage (durations: seq<string * int>) = 
    durations 
    |> Seq.map (fun (s,ms) -> double ms)
    |> Seq.average

let calcRequestAverages (durations : seq<Guid * string * int>) =
    durations 
    |> Seq.map (fun (guid,s,d) -> (s,d))
    |> Seq.groupBy (fun (s,d) -> s)
    |> Seq.map (fun (s,d) -> (s,calcMsAverage d))
