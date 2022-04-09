module Csv

open Types
open Timestamps
open System.IO
open System
open System.Threading

let deleteFile file =
    if (File.Exists file) then File.Delete (file)

let deleteCsvFiles() = 
    printfn "%s" Config.timestampsCsv
    printfn "%s" Config.durationsCsv 
    printfn "%s" Config.averagesCsv  
    printfn "%s" Config.matrixAvgCsv 
    deleteFile Config.timestampsCsv
    deleteFile Config.durationsCsv
    deleteFile Config.averagesCsv
    deleteFile Config.matrixAvgCsv

let fromTimestampCsvStr (str : string) = 
    let split = str.Split(";")
    fromSplitStr (Guid.Parse(split.[0])) split.[1] (Seq.skip 2 split)

let toTimestampCsvStr t = $"{t.id};{t.step};{t.service};{t.action};{formatDate t.date};{t.ms};{t.msSum};{t.prevDiff}"

/// <summary>
/// Appends timestamps to csv file.
/// Ignores timestamps where response.success is false.
/// </summary>
let appendTimestampsToCsv (mutex : Mutex) (response:ApiResult ) = 
    if (not response.success) then ()
    let timestampsStr = response.timestamps 
                        |> (parseRawTimestamps response.id response.step)
                        |> Seq.map toTimestampCsvStr
    mutex.WaitOne() |> ignore
    File.AppendAllLines(Config.timestampsCsv , timestampsStr)
    mutex.ReleaseMutex() 
    timestampsStr

let readTimestampsFromFile() =
    File.ReadAllLines(Config.timestampsCsv)
    |> Seq.cast<string>
    |> Seq.map fromTimestampCsvStr

let writeDurationsToCsv (durations: seq<Duration>) = 
    let lines = durations |> Seq.map (fun d -> $"{d.requestId};{d.step};{d.service};{d.duration};")
    File.WriteAllLines(Config.durationsCsv, lines)
    
let writeRequestAveragesToCsv (averages: seq<Average>) = 
    let str = averages |> Seq.map (fun a -> $"{a.step};{a.service};{a.average};")
    File.WriteAllLines(Config.averagesCsv, str)

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

    File.WriteAllLines(Config.matrixAvgCsv, allLines)

// Calculations

let find (timestamps: seq<Timestamp>) action service = 
    timestamps |> Seq.find (fun t -> t.action = action && t.service = service) 

let getDuration start ``end`` = 
    {
        requestId = start.id; 
        step = start.step; 
        service = start.service; 
        duration = getDurationDiff ``end``.date start.date
    }
    
let getServiceDurations (timestamps:seq<Timestamp>) = 
    let mutable durations = Seq.empty<Duration>
    let mutable start  : Timestamp option = None
    let mutable ``end``: Timestamp option = None
    
    for timestamp in timestamps do
        match (timestamp.action, start, ``end``) with 
            | ("Start", None, _) -> start    <- Some timestamp
            | ("End", Some _, None) -> ``end``  <- Some timestamp
            | _ -> ()
        match (start, ``end``) with
            | (Some s, Some e) -> 
                durations <- Seq.append durations [getDuration s e]
                start <- None
                ``end`` <- None
            | _ -> ()
    durations

let calcRequestDurations timestamps =
    timestamps
    |> Seq.groupBy (fun (timestamp) -> (timestamp.id, timestamp.service))
    |> Seq.map (fun (guid, t) -> getServiceDurations t)
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
