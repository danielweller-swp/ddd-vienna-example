module Application.PubSubForwarding.Routes

open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let signalHandler =
    let handler = fun signal -> taskResult {
        System.Console.Out.WriteLine("Forwarding signal via Pub/Sub")
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