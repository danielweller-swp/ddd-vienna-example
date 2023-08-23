module Common.SignalSubscription

open System.Text.Json
open System.Threading.Tasks
open Aggregation.Contracts
open Aggregation.Contracts.Signals.V1
open FsToolkit.ErrorHandling
open Giraffe
open Microsoft.AspNetCore.Http

type PubSubMessage = {
    Data: string
}

type PubSubEvent = {
    Message: PubSubMessage
}

let httpHandler (handler: Signal -> TaskResult<unit, exn>) = fun next (ctx: HttpContext) -> task {
    let! body = ctx.ReadBodyFromRequestAsync()
    let event = JsonSerializer.Deserialize<PubSubEvent>(body)
    
    let signal =
        event.Message.Data
        |> System.Convert.FromBase64String
        |> System.Text.Encoding.UTF8.GetString
        |> Signals.V1.deserialize
    
    let! result = signal |> handler
    
    match result with
    | Ok _ -> return! json {|  |} next ctx
    | Error e -> return! ServerErrors.INTERNAL_ERROR e.Message next ctx
}