module Aggregation.Application.Domain.AggregationScheduler

open System
open System.Threading
open System.Threading.Tasks
open Application.Bus

open Application.Domain
open Application.Domain.Providers
open Microsoft.Extensions.Hosting
open Aggregation.Contracts.Signals.V1
open NodaTime

let SIGNAL_TOPIC = "aggregation-signals" |> TopicIdentifier

let publishSignal (bus: IBus) clock signal =
    signal
    |> serialize
    |> bus.Publish SIGNAL_TOPIC
    
    
let aggregateAndPublishSignals clock bus (providers: IProvider list) = task {
    let! signalResults =
        providers
        |> List.map(fun provider ->
            provider.GetSignal()
            )
        |> Task.WhenAll
        
    let signals =
        signalResults
        |> Array.collect(fun result ->
            match result with
            | Ok signal ->
                [| signal |]
            | Error e ->
                Console.Out.WriteLine($"Error aggregating signal: {e.Message}")
                [|  |]
            )
        
    let! publishResults =               
        signals
        |> Array.map(fun signal ->
            signal
            |> SignalValidation.validate
            |> publishSignal bus clock
            )
        |> Task.WhenAll
        
    publishResults
    |> Array.iter(fun result ->
        match result with
        | Error e ->
            Console.Out.WriteLine($"Error publishing signal: {e.Message}")
        | _ -> ()
        )        
}


type AggregateFromProvidersService(clock: IClock, bus: IBus, providers: IProvider list) =
    let mutable timer = None
    
    interface IHostedService with
        member _.StartAsync token =        
            let callback = TimerCallback (fun _ ->
                let task = aggregateAndPublishSignals clock bus providers
                task.Wait()
                )
            timer <- new Timer(callback, null, 1000, 10000) |> Some
            
            Task.CompletedTask

        member _.StopAsync token =
            match timer with
            | None -> ()
            | Some x -> x.Dispose()
            
            Task.CompletedTask
