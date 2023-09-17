module Application.WebhookForwarding.Options

open System
open Microsoft.Extensions.DependencyInjection

type Webhook = {
    Url: Uri
    Key: string
}

type BatchingConfiguration = {
    BatchSize: int
}
with
    static member Default = { BatchSize = 10 }
    
type BatchingFeature =    
    | NoBatching
    | Batching of BatchingConfiguration

type Features = {
    Batching: BatchingFeature
}

type ApplicationConfiguration = {
    Webhooks: Webhook list
    Features: Features
}

let parseBool (str: string) =
    match str.ToUpperInvariant() with
    | "TRUE" -> true
    | _ -> false    

let buildConfiguration (services: IServiceProvider) : ApplicationConfiguration =
    let batching =
        match Environment.GetEnvironmentVariable("USE_BATCHING") |> parseBool with
        | true -> BatchingConfiguration.Default |> Batching
        | false -> NoBatching
    
    {
        Webhooks = [
            {
                Url = Uri("https://customer.com/webhook")
                Key = Environment.GetEnvironmentVariable("CUSTOMER_1_WEBHOOK_KEY")
            }
        ]
        Features = {
            Batching = batching
        }
    }

let configureOptions (services: IServiceCollection) =
    services.AddSingleton<ApplicationConfiguration>(buildConfiguration)