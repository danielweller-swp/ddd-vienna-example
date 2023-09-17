module Application.WebhookForwarding.Routes

open Giraffe
open Microsoft.AspNetCore.Http

open Application.WebhookForwarding.SignalHandler

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