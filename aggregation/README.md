# Aggregation

The purpose of the **aggregation** BC is to aggregate 
GPS signals from various sources, and to provide
these signals to other BCs in the signals domain.

## Local Development

### Infrastructure Dependencies

This bounded context has infrastructure dependencies (e.g. PostgreSQL).
To make them available when running locally, start them via Docker using

```bash
./run-infra-deps.sh
```

### Build

```bash
dotnet build
```

### Test

```bash
dotnet test
```

### Run

```bash
export AGGREGATION_SIGNALS_TOPIC=aggregation-signals
dotnet run --project src/Application
```

### Sending a command

```bash
curl -k -X POST https://localhost:5001/aggregate-from-providers
<<<<<<< HEAD
```

## Publishing a new Contracts library version

The `Aggregation.Contracts` package is consumed by other BCs as a
NuGet package. To publish a new version of the package, make sure
that access to the NuGet registry is setup according to
[the docs](../README.md#package-repository). Then do

```bash
export VERSION=1.0.0

cd src/Contracts

dotnet pack --configuration Release
dotnet nuget push "bin/Release/Aggregation.Contracts.${VERSION}.nupkg" --source "github"
