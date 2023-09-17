namespace Aggregation.Contracts

module Instant =
    open NodaTime
    open NodaTime.Text
    open Thoth.Json.Net
    
    let instantEncoder (instant: Instant) =
        instant
        |> InstantPattern.ExtendedIso.Format
        |> Encode.string
    
    let instantDecoder : Decoder<Instant> =
        Decode.string
        |> Decode.andThen(fun str ->
            let result = str |> InstantPattern.ExtendedIso.Parse
            if result.Success then
                Decode.succeed result.Value
            else
                Decode.fail $"Could not parse instant: {result.Exception}"
        )
