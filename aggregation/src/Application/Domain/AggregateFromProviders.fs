module Aggregation.Application.Domain.AggregateFromProviders

open System
open System.Threading.Tasks
open Application.Bus

open Application.Domain
open Application.Domain.Providers
open Microsoft.AspNetCore.Http
open Aggregation.Contracts.Signals.V1
open NodaTime

open Giraffe

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

let handler = fun (next: HttpFunc) (ctx: HttpContext) -> task {
    let clock = ctx.GetService<IClock>()
    let bus = ctx.GetService<IBus>()
    let providers = ctx.GetService<IProvider list>()
    do! aggregateAndPublishSignals clock bus providers
    return! json {|  |} next ctx
}
