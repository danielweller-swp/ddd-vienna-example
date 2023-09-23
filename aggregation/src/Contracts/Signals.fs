﻿namespace Aggregation.Contracts

module Signals =
    module V1 =

        open System
        open System.Text.Json
        open System.Text.Json.Serialization
        open NodaTime
        open NodaTime.Serialization.SystemTextJson

        type ValidationResult =
            | Valid
            | Invalid of string

        type Signal = {
            Latitude: decimal
            Longitude: decimal
            Timestamp: Instant
            ValidationResult: ValidationResult
            ProviderId: Guid
        }
        
        let setupJsonOptions() =
            let options = JsonSerializerOptions()

            options
                .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb) |> ignore

            options |>
            JsonFSharpOptions
                .Default()
                .AddToJsonSerializerOptions

            options
            
        let jsonOptions = setupJsonOptions()
            
        let deserialize (signalJson: string) : Signal =
            JsonSerializer.Deserialize<Signal>(signalJson, jsonOptions)
            
        let serialize (signal: Signal) : string =
            JsonSerializer.Serialize<Signal>(signal, jsonOptions)