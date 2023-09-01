module Application.WebhookForwarding.Routes

open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let signalHandler =
    let handler = fun signal -> taskResult {
        System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
    } 
    Common.SignalSubscription.httpHandler handler

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> text "Forwarding BC (Webhook)"
            ]
        POST >=>
            choose [
                route "/" >=> signalHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]