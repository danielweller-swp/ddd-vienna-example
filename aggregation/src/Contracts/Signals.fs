namespace Aggregation.Contracts

module Signals =
    module V1 =

        open System.Text.Json
        open NodaTime
        open NodaTime.Serialization.SystemTextJson

        type Signal = {
            Latitude: decimal
            Longitude: decimal
            Timestamp: Instant
        }
        
        let setupJsonOptions() =
            let options = JsonSerializerOptions()
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb) |> ignore
            options
            
        let jsonOptions = setupJsonOptions()
            
        let deserialize (signalJson: string) : Signal =
            JsonSerializer.Deserialize<Signal>(signalJson, jsonOptions)
            
        let serialize (signal: Signal) : string =
            JsonSerializer.Serialize<Signal>(signal, jsonOptions)            