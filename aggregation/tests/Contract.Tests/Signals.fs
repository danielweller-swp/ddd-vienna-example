namespace Contract.Tests

open FsCheck.Xunit;
open FsCheck;
open Aggregation.Contracts;

type NodaTime =
    static member Instant() =
        Arb.Default.DateTimeOffset()
        |> Arb.convert NodaTime.Instant.FromDateTimeOffset (fun d -> d.ToDateTimeOffset())

type NonEmptyString =
    static member NonEmptyString() =
        Arb.Default.NonEmptyString()
        |> Arb.convert
            (fun (nonEmptyString: FsCheck.NonEmptyString) -> nonEmptyString.Get)
            FsCheck.NonEmptyString

[<Properties( Arbitrary=[| typeof<NodaTime>; typeof<NonEmptyString> |] )>]
module Signals =
    [<Property>]
    let ``Signal encoding and decoding works`` (signal:  Signals.V2.Signal) =
        let deserialized = 
            signal
            |> Signals.V2.Encoding.serialize 
            |> Signals.V2.Encoding.deserialize
        deserialized = Ok signal
