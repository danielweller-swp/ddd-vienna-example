module Application.WebhookForwarding.SignalHandler

open System.Threading.Tasks
open Aggregation.Contracts.Signals.V1
open Application.WebhookForwarding.Options
open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

// TODO: this is for late
let handleSignalWithoutBatching webhook signal =
    System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
    System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")

// TODO: this is for late
let handleSignalWithBatching batchingConfig webhook signal =
    System.Console.Out.WriteLine($"Batching signal {signal} until {batchingConfig.BatchSize} is reached")
    System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")

let signalHandler : (HttpFunc -> HttpContext -> Task<HttpContext option>) =
    let handler = fun (ctx: HttpContext) (signal: Signal) -> taskResult {
        let config = ctx.GetService<ApplicationConfiguration>()
        config.Webhooks
        |> List.iter(fun webhook ->
            // TODO: this is for early
            //System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
            //System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")

            // TODO: this is for late
            match config.Features.Batching with
            | NoBatching -> signal |> handleSignalWithoutBatching webhook
            | Batching batchingConfig -> signal |> handleSignalWithBatching batchingConfig webhook
            )
        
    } 
    Common.SignalSubscription.httpHandler handler