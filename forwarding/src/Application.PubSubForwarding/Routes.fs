module Application.PubSubForwarding.Routes

open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let sendSignalViaPubSub signal =
    Ok ()

let signalHandler =
    let handler = fun ctx signal -> taskResult {
        match sendSignalViaPubSub signal with
        | Ok _ ->
            // This log-line is used for a log-based metric!
            System.Console.Out.WriteLine($"Forwarded signal via Pub/Sub: {signal}")
        | Error e ->
            // This log-line is used for a log-based metric!
            System.Console.Out.WriteLine($"Error forwarding signal via Pub/Sub: {e}")
    } 
    Common.SignalSubscription.httpHandler handler

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> text "Forwarding BC (Pub/Sub)"
            ]
        POST >=>
            choose [
                route "/" >=> signalHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]    