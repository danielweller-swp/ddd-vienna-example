module Application.WebhookForwarding.Routes

open Application.WebhookForwarding.Options
open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let signalHandler =
    let handler = fun (ctx: HttpContext) signal -> taskResult {
        let config = ctx.GetService<ApplicationConfiguration>()
        config.Webhooks
        |> List.iter(fun webhook ->
            System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
            System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")
            )
        
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