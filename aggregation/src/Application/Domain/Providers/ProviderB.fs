module Application.Domain.Providers.ProviderB

open System
open NodaTime
open FsToolkit.ErrorHandling

let providerId = Guid.Parse("472cdf1e-d222-4e9a-b7eb-164e67d044ff") |> ProviderId

let randomInvalidSignal(clock: IClock) =
    let rnd = Random()

    {
        Latitude = rnd.NextDouble() * 91.0 + 90.0 |> decimal
        Longitude = rnd.NextDouble() * 181.0 + 180.0 |> decimal
        Timestamp = clock.GetCurrentInstant()
    }
    
type ProviderB(clock: IClock) =
    interface IProvider with
        member this.GetSignal() = taskResult {
            return randomInvalidSignal(clock)
        }

        member this.ProviderId = providerId