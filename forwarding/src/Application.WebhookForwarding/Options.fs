module Application.WebhookForwarding.Options

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

type Webhook = {
    Url: Uri
    Key: string
}

type ApplicationConfiguration = {
    Webhooks: Webhook list
}

let buildConfiguration (services: IServiceProvider) : ApplicationConfiguration =
    {
        Webhooks = [
            {
                Url = Uri("https://customer.com/webhook")
                Key = Environment.GetEnvironmentVariable("CUSTOMER_1_WEBHOOK_KEY")
            }
        ]
    }

let configureOptions (services: IServiceCollection) =
    services.AddSingleton<ApplicationConfiguration>(buildConfiguration)