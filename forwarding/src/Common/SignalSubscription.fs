module Common.SignalSubscription

open System.Text.Json
open FsToolkit.ErrorHandling
open Giraffe
open Microsoft.AspNetCore.Http

type PubSubMessage = {
    Data: string
}

type PubSubEvent = {
    Message: PubSubMessage
}

let buildJsonOptions () =
    let options = JsonSerializerOptions()
    options.PropertyNameCaseInsensitive <- true
    options
    
let jsonOptions = buildJsonOptions()

let deserializePubSubEvent (str: string) = JsonSerializer.Deserialize<PubSubEvent>(str, jsonOptions)

let httpHandler (handler: HttpContext -> Aggregation.Contracts.Signals.V2.Signal -> TaskResult<unit, exn>) =
    fun next (ctx: HttpContext) -> task {
        let! body = ctx.ReadBodyFromRequestAsync()
        let event = body |> deserializePubSubEvent

        let signalResult =
            event.Message.Data
            |> System.Convert.FromBase64String
            |> System.Text.Encoding.UTF8.GetString
            |> Aggregation.Contracts.Signals.V2.Encoding.deserialize

        match signalResult with
        | Ok signal ->
            let! result = signal |> handler ctx

            match result with
            | Ok _ -> return! json {|  |} next ctx
            | Error e -> return! ServerErrors.INTERNAL_ERROR e.Message next ctx
        | Error e ->
            return! ServerErrors.INTERNAL_ERROR e next ctx
    }
