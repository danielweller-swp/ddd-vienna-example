namespace Application.Domain.Providers

[<AutoOpen>]
module Providers =

    open System
    open FsToolkit.ErrorHandling
    open NodaTime

    type ProviderId = ProviderId of Guid

    module ProviderId =
        let value (ProviderId id) = id

    type ProviderSignal = {
        Latitude: decimal
        Longitude: decimal
        Timestamp: Instant
    }

    type IProvider =
        abstract GetSignal: unit -> TaskResult<ProviderSignal, exn>
        abstract ProviderId: ProviderId
