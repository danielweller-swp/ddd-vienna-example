module Application.Domain.Providers.ProviderA

open System
open NodaTime
open FsToolkit.ErrorHandling

let providerId = Guid.Parse("05da02cd-65f6-4688-81a2-19053c9dd0b4") |> ProviderId

let randomValidSignal(clock: IClock) =
    let rnd = Random()

    {
        Latitude = rnd.NextDouble() * 180.0 - 90.0 |> decimal
        Longitude = rnd.NextDouble() * 360.0 - 180.0 |> decimal
        Timestamp = clock.GetCurrentInstant()
    }
    
type ProviderA(clock: IClock) =
    interface IProvider with
        member this.GetSignal() = taskResult {
            return randomValidSignal(clock)
        }

        member this.ProviderId = providerId