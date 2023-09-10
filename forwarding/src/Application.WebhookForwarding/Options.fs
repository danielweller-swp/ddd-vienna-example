module Application.WebhookForwarding.Options

open System
open Microsoft.Extensions.DependencyInjection

type Webhook = {
    Url: Uri
    Key: string
}

// TODO: this is for late
type BatchingConfiguration = {
    BatchSize: int
}
with
    static member Default = { BatchSize = 10 }
    
// TODO: this is for late
type BatchingFeature =    
    | NoBatching
    | Batching of BatchingConfiguration

// TODO: this is for late
type Features = {
    Batching: BatchingFeature
}

type ApplicationConfiguration = {
    Webhooks: Webhook list
    // TODO: this is for late    
    Features: Features
}

// TODO: this is for late
let parseBool (str: string) =
    match str.ToUpperInvariant() with
    | "TRUE" -> true
    | _ -> false    

let buildConfiguration (services: IServiceProvider) : ApplicationConfiguration =
    // TODO: this is for late
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
        // TODO: this is for late
        Features = {
            Batching = batching
        }
    }

let configureOptions (services: IServiceCollection) =
    services.AddSingleton<ApplicationConfiguration>(buildConfiguration)