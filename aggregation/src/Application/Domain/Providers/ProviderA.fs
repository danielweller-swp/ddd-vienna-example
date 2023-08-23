module Application.Domain.Providers.ProviderA

open System
open NodaTime
open FsToolkit.ErrorHandling

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
