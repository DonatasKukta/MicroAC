module Csv

open Types
open Timestamps
open System.IO
open System
open System.Threading

let printProcessingMessage message =
    printfn "%A - %s" DateTime.Now message

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
    File.AppendAllLines(Config.timestampsCsv() , timestampsStr)
    mutex.ReleaseMutex() 
    timestampsStr

let readTimestampsFromFile() =
    File.ReadAllLines(Config.timestampsCsv())
    |> Seq.cast<string>
    |> Seq.map fromTimestampCsvStr

let writeDurationsToCsv (durations: seq<Duration>) = 
    printProcessingMessage "Write durations to csv"
    let lines = durations |> Seq.map (fun d -> $"{d.requestId};{d.step};{d.service};{d.duration};")
    File.WriteAllLines(Config.durationsCsv(), lines)
    
let writeRequestAveragesToCsv (averages: seq<Average>) = 
    printProcessingMessage "Write averages to csv"
    let str = averages |> Seq.map (fun a -> $"{a.step};{a.service};{a.average};")
    File.WriteAllLines(Config.averagesCsv(), str)

let writeTestTimesToCsv (started:DateTime) completed = 
    printProcessingMessage "Write Run times to csv"
    File.WriteAllLines(Config.resultsCsv(), [
        $"Start;{started}";
        $"End;{completed }";
        $"Duration;{(completed - started)}";
        ])

let appendMetricsToCsv (externalRequestCount, internalRequestCount, nodeCounts, timestampCount) =
    printProcessingMessage "Write metrics to csv"
    let allLines = 
        nodeCounts
        |> Seq.map (fun (node, count) -> $"{node};{count}")
        |> Seq.append [ 
            $"LoadSize;{Config.testLoadSize}";
            $"LoadDuration;{Config.testDuration}";
            $"CentralAuth;{Config.centralAuthEnabled};;ReverseProxy;{Config.reverseProxyEnabled}";
            $"Timestamps;{timestampCount}";
            $"ExternalRequests;{externalRequestCount}";  
            $"InternalRequests;{internalRequestCount}";
            "";
            ]
    File.AppendAllLines(Config.resultsCsv(), allLines)

let appendServiceRequestCountsToCsv (timestamps: seq<Timestamp>) = 
    File.AppendAllLines(Config.resultsCsv(), 
        timestamps
        |> Seq.filter (fun t -> t.action = "Start")
        |> Seq.map (fun t -> t.service)
        |> Seq.countBy (fun s -> s)
        |> Seq.map(fun (service ,count) -> $"{service};{count}")
        |> Seq.append [ ""; ""; ""; "Service;Count"; ] 
    )

let appendCalcAverageMatrixToCsv averages =
    printProcessingMessage "Append averages matrix to csv"
    let rows    = averages |> Seq.map (fun a -> a.step)    |> Seq.distinct |> Seq.cache
    let columns = averages |> Seq.map (fun a -> a.service) |> Seq.distinct |> Seq.cache
    let matrix = Array2D.zeroCreate (Seq.length rows)  (Seq.length columns)
    
    let getRow step = Seq.findIndex (fun s -> s.Equals(step)) rows
    let getCol col  = Seq.findIndex (fun c -> c.Equals(col)) columns

    Seq.iter (fun a -> matrix.[getRow a.step, getCol a.service] <- a.average) averages
    
    let rowStr row = Seq.fold (fun f a -> $"{f};{a}") "" matrix.[row,*]
    let strLines = seq{0..1..(Seq.length rows)-1}
                    |> Seq.map (fun i -> $"{Seq.item i rows}{rowStr i};")
    let firstLine = Seq.fold (fun f r -> $"{f}{r};") ";" columns
    let allLines = strLines |> Seq.append [firstLine]
    
    File.AppendAllLines(Config.resultsCsv(), allLines)
    printProcessingMessage "Done"

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

let calcMetrics (timestamps: seq<Timestamp>) =
    let timestampCount = Seq.length timestamps
    let externalRequestCount = timestamps
                                |> Seq.distinctBy (fun t -> t.id) 
                                |> Seq.length
    let internalRequestCount = timestamps
                                |> Seq.filter (fun t -> t.action.Equals("Start"))
                                |> Seq.groupBy (fun t -> (t.id, t.service, t.step))  
                                |> Seq.length
    let nodeCounts = timestamps
                        |> Seq.filter (fun t -> t.action.Contains("_Node_"))
                        |> Seq.map (fun t -> t.action)
                        |> Seq.countBy (fun a -> a)
    
    (externalRequestCount, internalRequestCount, nodeCounts, timestampCount)
