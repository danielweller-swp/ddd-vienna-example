module Aggregation.Application.AggregationScheduler

open System
open System.Threading
open System.Threading.Tasks
open Application
open Microsoft.Extensions.Hosting
open Aggregation.Contracts.Signals.V1
open NodaTime

let randomSignal(clock: IClock) =
    let rnd = Random()
    
    {
        Latitude = rnd.NextDouble() * 180.0 - 90.0 |> decimal
        Longitude = rnd.NextDouble() * 360.0 - 180.0 |> decimal
        Timestamp = clock.GetCurrentInstant()
    }

let publishRandomSignal clock =
    randomSignal clock
    |> SignalPublisher.publishSignal

type AggregationSchedulerService(clock: IClock) =
    let mutable timer = None
    
    interface IHostedService with
        member _.StartAsync token =
            let callback = TimerCallback (fun _ -> publishRandomSignal clock)
            timer <- new Timer(callback, null, 1000, 10000) |> Some
            
            Task.CompletedTask
        member _.StopAsync token =
            match timer with
            | None -> ()
            | Some x -> x.Dispose()
            
            Task.CompletedTask
