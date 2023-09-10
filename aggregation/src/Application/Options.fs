module Application.Options

// TODO: this belongs to mid

open System
open Application.Bus
open Microsoft.Extensions.DependencyInjection

type ApplicationConfiguration = {
    SignalsTopicIdentifier: TopicIdentifier
}

let buildConfiguration (services: IServiceProvider) : ApplicationConfiguration =
    {
        SignalsTopicIdentifier = Environment.GetEnvironmentVariable("AGGREGATION_SIGNALS_TOPIC") |> TopicIdentifier
    }

let configureOptions (services: IServiceCollection) =
    services.AddSingleton<ApplicationConfiguration>(buildConfiguration)