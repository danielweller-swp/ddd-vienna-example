module Application.Domain.SignalValidation

open Aggregation.Contracts.Signals.V2
open Application.Domain.Providers

let validate (providerSignal: ProviderSignal) : Signal =
    let validationResult =
        match providerSignal.Latitude, providerSignal.Longitude with
        | lat, _ when lat < -90m || 90m < lat ->
            "Latitude outside of [-90, 90]"
            |> Invalid 
        | _, lon when lon < -180m || 180m < lon ->
            "Longitude outside of [-180, 180]"
            |> Invalid
        | _, _ -> Valid
    {
        Latitude = providerSignal.Latitude
        Longitude = providerSignal.Longitude
        Timestamp = providerSignal.Timestamp
        ValidationResult = validationResult 
    }