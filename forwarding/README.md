# Forwarding

The purpose of the **forwarding** BC is to forward 
signals received from the aggregation BC to customers.

## Deployables

The bounded context contains two deployables:

- `Application.PubSubForwarding`: Forward signals to customers via Pub/Sub.
- `Application.WebhookForwarding`: Forward signals to customers via Webhooks.

## Local Development

### Build

```bash
dotnet build
```

### Test

```bash
# TODO: tests
dotnet test
```

### Run
Each deployable can be run by changing to its directory in `src/`
and running
```bash
dotnet run
```

### Sending a signal

Both deployables expect Pub/Sub messages containing signals as data
delivered via POST to the root route:
```bash
export SIGNAL_JSON="
{
  \"Latitude\": 1.0,
  \"Longitude\": 2.0,
  \"Timestamp\": \"2023-09-01T07:17:11Z\",
  \"ValidationResult\": {
    \"Case\": \"Valid\"
  }
}"

curl -k -H "Content-Type: application/json" -d \
"{
  \"message\": {
    \"data\": \"$(echo $SIGNAL_JSON | base64 -w 0)\"
  }
}" \
https://localhost:5001/
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

Build the docker image on GCP and deploy as Cloud Run:
```bash
gcloud builds submit --tag eu.gcr.io/$GCP_PROJECT/aggregation_application:$AGGREGATION_VERSION --project $GCP_PROJECT
```

Deploy as Cloud Run:
```bash
gcloud run deploy aggregation-application --image eu.gcr.io/$GCP_PROJECT/aggregation_application:$AGGREGATION_VERSION --region europe-west1
```

### One-Time Setup

# TODO: create a push subscription
