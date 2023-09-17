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
```

## Deployment

### Deploying a new app version

Build the release version:
```bash
dotnet build Aggregation.sln -c Release
```

Publish it and change directory:
```bash
dotnet publish src/Application/Application.fsproj --no-build -c Release -o artifacts/aggregation-application
cd artifacts/aggregation-application
``

Set the GCP project to deploy to:
```bash
export GCP_PROJECT=ddd-vienna-sample
```

Set the version:
```bash
export AGGREGATION_VERSION=`git rev-parse HEAD`
```

Build the docker image on GCP:
```bash
gcloud builds submit --tag eu.gcr.io/$GCP_PROJECT/aggregation_application:$AGGREGATION_VERSION --project $GCP_PROJECT
```

Deploy as Cloud Run:
```bash
gcloud run deploy aggregation-application --image eu.gcr.io/$GCP_PROJECT/aggregation_application:$AGGREGATION_VERSION --region europe-west1
```

### One-Time Setup

Create a scheduler job that triggers the aggregation every 10 minutes:
```bash
export GCP_PROJECT_NUMBER=642254565385
# This is the URL from last command from the previous section
export AGGREGATION_APPLICATION_BASE_URL=https://aggregation-application-4yyc3hq26q-ew.a.run.app

gcloud scheduler jobs create http aggregation-schedule \
  --location europe-west1 \
  --schedule="*/10 * * * *" \
  --uri="$AGGREGATION_APPLICATION_BASE_URL/aggregate-from-providers" \
  --http-method POST \
  --oidc-service-account-email=$GCP_PROJECT_NUMBER-compute@developer.gserviceaccount.com \
  --oidc-token-audience="$AGGREGATION_APPLICATION_BASE_URL"
```

### Publishing a new Contracts library version

The `Aggregation.Contracts` package is consumed by other BCs as a
NuGet package. To publish a new version of the package, make sure
that access to the NuGet registry is setup according to
[the docs](../README.md#package-repository). Then do

```bash
export VERSION=1.0.0

cd src/Contracts

dotnet pack --configuration Release
dotnet nuget push "bin/Release/Aggregation.Contracts.${VERSION}.nupkg" --source "github"
```