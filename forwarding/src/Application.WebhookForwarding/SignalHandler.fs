module Application.WebhookForwarding.SignalHandler

open System.Threading.Tasks
open Aggregation.Contracts.Signals.V2
open Application.WebhookForwarding.Options
open Giraffe
open Microsoft.AspNetCore.Http
open FsToolkit.ErrorHandling

let sendSignalToWebhook webhook signal =
    System.Console.Out.WriteLine($"Forwarding signal via Webhook: {signal}")
    Ok ()

let handleSignalWithoutBatching webhook signal =
    match sendSignalToWebhook webhook signal with
    | Ok _ ->
        // This log-line is used for a log-based metric!
        System.Console.Out.WriteLine($"Forwarded signal via Webhook: {signal}")
    | Error e ->
        // This log-line is used for a log-based metric!
        System.Console.Out.WriteLine($"Error forwarding signal via Webhook: {e}")

let batchSignal batchingConfig webhook signal =
    System.Console.Out.WriteLine($"Batching signal {signal} until {batchingConfig.BatchSize} is reached")
    Ok()

let handleSignalWithBatching batchingConfig webhook signal =
    match batchSignal batchingConfig webhook signal with
    | Ok _ ->
        // This log-line is used for a log-based metric!
        System.Console.Out.WriteLine($"Batched signal {signal}")
    | Error e ->
        // This log-line is used for a log-based metric!
        System.Console.Out.WriteLine($"Error batching signal: {e}")

let signalHandler : (HttpFunc -> HttpContext -> Task<HttpContext option>) =
    let handler = fun (ctx: HttpContext) (signal: Signal) -> taskResult {
        let config = ctx.GetService<ApplicationConfiguration>()
        config.Webhooks
        |> List.iter(fun webhook ->
            System.Console.Out.WriteLine($"Webhook: {webhook.Url}?key={webhook.Key}")
            match config.Features.Batching with
            | NoBatching -> signal |> handleSignalWithoutBatching webhook
            | Batching batchingConfig -> signal |> handleSignalWithBatching batchingConfig webhook
            )
        
    } 
    Common.SignalSubscription.httpHandler handler