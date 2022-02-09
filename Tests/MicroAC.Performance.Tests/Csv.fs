﻿module Csv

open Types
open Timestamps
open System.IO
open System
open System.Threading

type TimestampCsv = (Guid * Timestamp)

let timestampsCsv = "_timestamps.csv"
let durationsCsv  = "_durations.csv"
let averagesCsv   = "_averages.csv"
let matrixAvgCsv  = "_matrixAvg.csv" 

let deleteFile file =
    if (File.Exists file) then File.Delete file

let deleteCsvFiles() = 
    deleteFile timestampsCsv
    deleteFile durationsCsv
    deleteFile averagesCsv
    deleteFile matrixAvgCsv

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

let writeDurationsToCsv (durations: seq<Duration>) = 
    let lines = durations |> Seq.map (fun d -> $"{d.requestId};{d.step};{d.service};{d.duration};")
    File.WriteAllLines(durationsCsv, lines)
    
let writeRequestAveragesToCsv (averages: seq<Average>) = 
    let str = averages |> Seq.map (fun a -> $"{a.step};{a.service};{a.average};")
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
                         {requestId = s.id; step = s.step; service = s.service; duration = duration}   )

let calcRequestDurations timestamps =
    timestamps
    |> Seq.groupBy (fun (timestamp) -> timestamp.id)
    |> Seq.map (fun (guid, t) -> ( (getServiceDurations t(*(Seq.map (fun(g,t)-> t) gt)*))))
    |> Seq.concat
    
let calcMsAverage (durations: seq<Duration>) = 
    durations 
    |> Seq.map (fun d -> double d.duration)
    |> Seq.average

let calcRequestAverages (durations : seq<Duration>) =
    durations 
    |> Seq.groupBy (fun d -> (d.step, d.service))
    |> Seq.map (fun (ss,d) -> let step, service = ss
                              {step = step; service = service; average = calcMsAverage d})

let calcAverageMatrixToCsv averages =
    let rows    = Seq.map (fun a -> a.step)    averages |> Seq.distinct
    let columns = Seq.map (fun a -> a.service) averages |> Seq.distinct
    let matrix = Array2D.zeroCreate (Seq.length rows)  (Seq.length columns)
    
    let getRow step = Seq.findIndex (fun s -> s.Equals(step)) rows
    let getCol col  = Seq.findIndex (fun c -> c.Equals(col)) columns

    Seq.iter (fun a -> matrix.[getRow a.step, getCol a.service] <- a.average) averages
    
    let rowStr row = Seq.fold (fun f a -> $"{f};{a}") "" matrix.[row,*]
    let strLines = seq{0..1..(Seq.length rows)-1}
                    |> Seq.map (fun i -> $"{Seq.item i rows}{rowStr i};")
    let firstLine = Seq.fold (fun f r -> $"{f}{r};") ";" columns
    let allLines = strLines |> Seq.append [firstLine]

    File.WriteAllLines(matrixAvgCsv, allLines)
