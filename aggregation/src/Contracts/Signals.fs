namespace Aggregation.Contracts

module Signals =
    module V1 =

        open System.Text.Json
        open NodaTime

        type Signal = {
            Latitude: decimal
            Longitude: decimal
            Timestamp: Instant
        }
        
        let deserialize (signalJson: string) : Signal =
            JsonSerializer.Deserialize<Signal> signalJson
            
        let serialize (signal: Signal) : string =
            JsonSerializer.Serialize<Signal> signal            