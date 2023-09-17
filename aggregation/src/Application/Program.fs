module Aggregation.Application.Program

open System
open System.IO
open Application
open Application.Bus
open Application.Bus.InMemory
open Application.Bus.PubSub
open Application.Domain.Providers
open Application.Domain.Providers.ProviderA
open Application.Domain.Providers.ProviderB
open Application.Options
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open NodaTime

[<Literal>]
let GCP_PROJECT = "ddd-vienna-sample"

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let isDevelopment (services: IServiceProvider) =
    let env = services.GetService<IWebHostEnvironment>()
    env.IsDevelopment()

let configureApp (app : IApplicationBuilder) =
    (match isDevelopment app.ApplicationServices with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(Routes.routes)

let createBus (services: IServiceProvider) : IBus =
    match isDevelopment services with
    | true -> InMemoryBus()
    | false -> PubSubBus(GCP_PROJECT)

let configureProviders(services: IServiceProvider) : IProvider list =
    let clock = services.GetService<IClock>()
    [
        ProviderA(clock)
        ProviderB(clock)
    ]

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services |> configureOptions |> ignore
    services.AddSingleton<IClock>(NodaTime.SystemClock.Instance) |> ignore
    services.AddSingleton<IBus>(createBus) |> ignore
    services.AddSingleton<IProvider list>(configureProviders) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0