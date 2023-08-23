module Aggregation.Application.AggregationScheduler

open System
open System.Threading
open System.Threading.Tasks
open Application.Bus

open Microsoft.Extensions.Hosting
open Aggregation.Contracts.Signals.V1
open NodaTime

let SIGNAL_TOPIC = "aggregation-signals" |> TopicIdentifier

let randomSignal(clock: IClock) =
    let rnd = Random()

    let validationResult =
        match rnd.Next(2) with
        | 0 -> Valid
        | _ -> "This signal has an error" |> Invalid  

    {
        Latitude = rnd.NextDouble() * 180.0 - 90.0 |> decimal
        Longitude = rnd.NextDouble() * 360.0 - 180.0 |> decimal
        Timestamp = clock.GetCurrentInstant()
        ValidationResult = validationResult 
    }

let publishRandomSignal (bus: IBus) clock =
    randomSignal clock
    |> serialize
    |> bus.Publish SIGNAL_TOPIC
    

type AggregationSchedulerService(clock: IClock, bus: IBus) =
    let mutable timer = None
    
    interface IHostedService with
        member _.StartAsync token =
            let callback = TimerCallback (fun _ ->
                let task = publishRandomSignal bus clock
                task.Wait()
                )
            timer <- new Timer(callback, null, 1000, 10000) |> Some
            
            Task.CompletedTask
        member _.StopAsync token =
            match timer with
            | None -> ()
            | Some x -> x.Dispose()
            
            Task.CompletedTask
