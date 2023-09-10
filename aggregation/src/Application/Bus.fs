module Application.Bus

open FsToolkit.ErrorHandling

type TopicIdentifier =
    | TopicIdentifier of string

type IBus =
    abstract Publish: TopicIdentifier -> string-> TaskResult<unit, exn>

module PubSub =
    open Google.Cloud.PubSub.V1
    
    type PubSubBus(gcpProjectId: string) =
        interface IBus with
            member this.Publish (TopicIdentifier topicId) message =
                task {
                    let topic = TopicName(gcpProjectId, topicId)
                    let publisher = PublisherClient.Create(topic)
                    try
                        System.Console.Out.WriteLine($"Publishing {message} to Pub/Sub")
                        
                        let! _ = publisher.PublishAsync message

                        System.Console.Out.WriteLine("Publishing successful.")
                        return () |> Ok
                    with
                    | e -> return e |> Error
                }

module InMemory =
    open System.Collections.Concurrent
    
    let addToMap topic (map: ConcurrentDictionary<TopicIdentifier, string list>) message =
        let add = fun _ -> []
        let update = fun _ messages -> message :: messages
        map.AddOrUpdate(topic, add, update) |> ignore
        
    
    type InMemoryBus() =
        let messages : ConcurrentDictionary<TopicIdentifier, string list> = ConcurrentDictionary()

        member this.MessagesOn topicId =
            match messages.TryGetValue(topicId) with
            | true, messages -> messages
            | false, _ -> []

        interface IBus with
            member this.Publish topicId message = taskResult {
                System.Console.Out.WriteLine($"Publishing {message} in-memory")

                message
                |> addToMap topicId messages

                System.Console.Out.WriteLine("Publishing successful.")
            }
