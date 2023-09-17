module Application.WebhookForwarding.SignalHandler

open System.Threading.Tasks
open Aggregation.Contracts.Signals.V2
open Application.WebhookForwarding.Options
open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let handleSignalWithoutBatching webhook signal =
    System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
    System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")

let handleSignalWithBatching batchingConfig webhook signal =
    System.Console.Out.WriteLine($"Batching signal {signal} until {batchingConfig.BatchSize} is reached")
    System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")

let signalHandler : (HttpFunc -> HttpContext -> Task<HttpContext option>) =
    let handler = fun (ctx: HttpContext) (signal: Signal) -> taskResult {
        let config = ctx.GetService<ApplicationConfiguration>()
        config.Webhooks
        |> List.iter(fun webhook ->
            match config.Features.Batching with
            | NoBatching -> signal |> handleSignalWithoutBatching webhook
            | Batching batchingConfig -> signal |> handleSignalWithBatching batchingConfig webhook
            )
        
    } 
    Common.SignalSubscription.httpHandler handler