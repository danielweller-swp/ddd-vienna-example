namespace Application.Domain.Providers

[<AutoOpen>]
module Providers =

    open FsToolkit.ErrorHandling
    open NodaTime

    type ProviderSignal = {
        Latitude: decimal
        Longitude: decimal
        Timestamp: Instant        
    }

    type IProvider =
        abstract GetSignal: unit -> TaskResult<ProviderSignal, exn>
