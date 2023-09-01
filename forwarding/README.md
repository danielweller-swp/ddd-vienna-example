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
dotnet build Forwarding.sln -c Release
```

Publish the deployables and change directory:
```bash
dotnet publish src/Application.PubSubForwarding/Application.PubSubForwarding.fsproj --no-build -c Release -o artifacts/forwarding-pubsub
dotnet publish src/Application.WebhookForwarding/Application.WebhookForwarding.fsproj --no-build -c Release -o artifacts/forwarding-webhook
cd artifacts
``

Set the GCP project to deploy to:
```bash
export GCP_PROJECT=ddd-vienna-sample
```

Set the version:
```bash
export FORWARDING_VERSION=`git rev-parse HEAD`
```

Build the docker images on GCP:
```bash
cd forwarding-pubsub
gcloud builds submit --tag eu.gcr.io/$GCP_PROJECT/forwarding_pubsub:$FORWARDING_VERSION --project $GCP_PROJECT

cd ../forwarding-webhook
gcloud builds submit --tag eu.gcr.io/$GCP_PROJECT/forwarding_webhook:$FORWARDING_VERSION --project $GCP_PROJECT
```

Deploy as Cloud Run:
```bash
gcloud run deploy forwarding-pubsub --image eu.gcr.io/$GCP_PROJECT/forwarding_pubsub:$FORWARDING_VERSION --region europe-west1
gcloud run deploy forwarding-webhook --image eu.gcr.io/$GCP_PROJECT/forwarding_webhook:$FORWARDING_VERSION --region europe-west1
```

### One-Time Setup

Find the *service URLs* of the Cloud Run deployables: they are output
by the `gcloud run deploy` commands, and can also be found via the
[GCP console](https://console.cloud.google.com/run?referrer=search&project=ddd-vienna-sample).

For each service URL, create a Pub/Sub subscription:
```bash
export GCP_PROJECT_NUMBER=642254565385

gcloud pubsub subscriptions create forwarding-pubsub-signals \
    --topic=aggregation-signals \
    --push-auth-service-account=$GCP_PROJECT_NUMBER-compute@developer.gserviceaccount.com \
    --push-endpoint=<FORWARDING_PUBSUB_SERVICE_URL>

gcloud pubsub subscriptions create forwarding-webhook-signals \
    --topic=aggregation-signals \
    --push-auth-service-account=$GCP_PROJECT_NUMBER-compute@developer.gserviceaccount.com \
    --push-endpoint=<FORWARDING_WEBHOOK_SERVICE_URL>    
```
