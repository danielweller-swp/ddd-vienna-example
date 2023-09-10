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
