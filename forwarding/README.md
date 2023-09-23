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