module Aggregation.Application.Domain.AggregateFromProviders

open System
open System.Threading.Tasks
open Application.Bus

open Application.Domain
open Application.Domain.Providers
open Application.Options
open Microsoft.AspNetCore.Http
open Aggregation.Contracts.Signals.V2.Encoding

open Giraffe

// TODO: this belongs to early
//let SIGNAL_TOPIC = "aggregation-signals" |> TopicIdentifier

let publishSignal (bus: IBus) topic signal =
    signal
    |> serialize
    |> bus.Publish topic
    
    
let aggregateAndPublishSignals bus topic (providers: IProvider list) = task {
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
            |> publishSignal bus topic
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
    let bus = ctx.GetService<IBus>()
    let providers = ctx.GetService<IProvider list>()
    // TODO: this belongs to mid
    let config = ctx.GetService<ApplicationConfiguration>()
    // TODO: this belongs to early
    // do! aggregateAndPublishSignals clock bus SIGNAL_TOPIC providers
    do! aggregateAndPublishSignals bus config.SignalsTopicIdentifier providers
    return! json {|  |} next ctx
}
