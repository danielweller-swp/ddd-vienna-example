module Application.SignalPublisher

open Aggregation.Contracts.Signals.V1
open Google.Cloud.PubSub.V1

let topic = TopicName("ddd-vienna-sample", "aggregation-signals")

let publishSignal (signal: Signal) =
    let publisher = PublisherClient.Create(topic)

    System.Console.Out.WriteLine($"Publishing {signal}")
    let task =
        signal
        |> serialize
        |> publisher.PublishAsync

    task.Wait()
    System.Console.Out.WriteLine("Done publishing.")