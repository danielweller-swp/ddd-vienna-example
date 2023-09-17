namespace Aggregation.Contracts

module Signals =
    module V2 =
        open NodaTime

        type ValidationResult =
            | Valid
            | Invalid of string

        type Signal = {
            Latitude: decimal
            Longitude: decimal
            Timestamp: Instant
            ValidationResult: ValidationResult
        }

        module Encoding =
            open Thoth.Json.Net
        
            [<Literal>]
            let ValidEncoding = "valid"
            
            [<Literal>]
            let InvalidEncoding = "invalid"
            
            let validationResultEncoder (validationResult: ValidationResult) =
                match validationResult with
                | Valid ->
                    [
                        "status", Encode.string ValidEncoding
                    ]
                | Invalid error ->
                    [
                        "status", Encode.string InvalidEncoding
                        "error", Encode.string error
                    ]
                |> Encode.object
            
            let validationResultDecoder : Decoder<ValidationResult> = 
                Decode.object (fun get ->
                    match get.Required.Field "status" Decode.string with
                    | ValidEncoding ->
                        Valid
                    | InvalidEncoding ->
                        let validationError = get.Required.Field "error" Decode.string
                        Invalid validationError
                    | x -> failwith $"Unknown validationResult {x}")

            let extra =
                Extra.empty
                |> Extra.withDecimal
                |> Extra.withCustom validationResultEncoder validationResultDecoder
                |> Extra.withCustom Instant.instantEncoder Instant.instantDecoder

            let deserialize (signalJson: string) : Result<Signal, string> =
                Decode.Auto.fromString(signalJson, CaseStrategy.CamelCase, extra)
                
            let serialize (signal: Signal) : string =
                Encode.Auto.toString(4, signal, CaseStrategy.CamelCase, extra)