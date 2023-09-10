module Application.Tests.AggregateFromProvidersTest

open Xunit
open NodaTime.Testing

module Fixtures =

    open Application.Bus
    open Application.Bus.InMemory
    open Application.Domain.Providers
    open Application.Domain.Providers.ProviderA
    open Application.Domain.Providers.ProviderB
    open NodaTime

    let clock = FakeClock(Instant.FromUtc(2023, 8, 23, 8, 0))
    let providers : IProvider list = [
        ProviderA(clock)
        ProviderB(clock)
    ]
    let bus = InMemoryBus()
    
    let signalsTopic = "aggregation-signals" |> TopicIdentifier
    
module When =
    open Aggregation.Application.Domain.AggregateFromProviders
    let aggregatingAndPublishingSignals () =
        aggregateAndPublishSignals Fixtures.bus Fixtures.signalsTopic Fixtures.providers

module Then =
    let aMessageShouldBePublishedOn topic =
        Fixtures.bus.MessagesOn topic
        |> Assert.NotEmpty 
        
    
module AggregateFromProviders =

    

    [<Fact>]
    let ``should publish at least one signal`` () = task {
        do! When.aggregatingAndPublishingSignals()
        Then.aMessageShouldBePublishedOn Fixtures.signalsTopic
    }