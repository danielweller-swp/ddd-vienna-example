# Aggregation

The purpose of the **notifications** BC is to inform
customers about problems related to their signals.

## Local Development

### Install Dependencies

```bash
npm ci
```

### Test

```bash
npm test
```

### Run

```bash
npm start
```

### Sending a signal

```bash
export SIGNAL_JSON="
{
  \"Latitude\": 1.0,
  \"Longitude\": 2.0,
  \"Timestamp\": \"2023-09-01T07:17:11Z\",
  \"ValidationResult\": {
    \"Case\": \"Invalid\",
    \"Fields\": [\"This signal is invalid\"]
  }
}"

curl -H "Content-Type: application/json" -d \
"{
  \"message\": {
    \"data\": \"$(echo $SIGNAL_JSON | base64 -w 0)\"
  }
}" \
http://localhost:8080/
```

## Deployment

### Deploying a new app version

Set the GCP project to deploy to:
```bash
export GCP_PROJECT=ddd-vienna-sample
```

Set the version:
```bash
export NOTIFICATIONS_VERSION=`git rev-parse HEAD`
```

Build the docker image on GCP:
```bash
gcloud builds submit --tag eu.gcr.io/$GCP_PROJECT/notifications_application:$NOTIFICATIONS_VERSION --project $GCP_PROJECT
```

Deploy as Cloud Run:
```bash
gcloud run deploy notifications-application --image eu.gcr.io/$GCP_PROJECT/notifications_application:$NOTIFICATIONS_VERSION --region europe-west1
```

### One-Time Setup

Find the *service URL* of the Cloud Run deployable: it is output
by the `gcloud run deploy` command, and can also be found via the
[GCP console](https://console.cloud.google.com/run?referrer=search&project=ddd-vienna-sample).

Then, create a Pub/Sub subscription:
```bash
export GCP_PROJECT_NUMBER=642254565385

gcloud pubsub subscriptions create notifications-signals \
    --topic=aggregation-signals \
    --push-auth-service-account=$GCP_PROJECT_NUMBER-compute@developer.gserviceaccount.com \
    --push-endpoint=<NOTIFICATIONS_SERVICE_URL>
```
