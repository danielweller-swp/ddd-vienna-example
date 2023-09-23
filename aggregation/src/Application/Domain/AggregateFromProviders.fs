module Aggregation.Application.Domain.AggregateFromProviders

open System
open System.Threading.Tasks
open Application.Bus

open Application.Domain
open Application.Domain.Providers
open Application.Options
open Microsoft.AspNetCore.Http
open Aggregation.Contracts.Signals.V1

open Giraffe
open FsToolkit.ErrorHandling

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
        
    let providersAndSignalResults =
        signalResults
        |> List.ofArray
        |> List.zip providers

    let signals =
        providersAndSignalResults
        |> List.collect(fun (provider, result) ->
            match result with
            | Ok signal ->
                [ provider, signal ]
            | Error e ->
                Console.Out.WriteLine($"Error aggregating signal: {e.Message}")
                [  ]
            )
        
    let! publishResults =               
        signals
        |> List.map(fun (provider, providerSignal) ->
            let validationResult =
                providerSignal
                |> SignalValidation.validate
            let signal = {
                    Latitude = providerSignal.Latitude
                    Longitude = providerSignal.Longitude
                    Timestamp = providerSignal.Timestamp
                    ProviderId = provider.ProviderId |> ProviderId.value
                    ValidationResult = validationResult
                }
            signal
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
    let config = ctx.GetService<ApplicationConfiguration>()
    do! aggregateAndPublishSignals bus config.SignalsTopicIdentifier providers
    return! json {|  |} next ctx
}
