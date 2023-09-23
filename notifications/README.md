# Notifications

The purpose of the **notifications** BC is to inform
customers about problems related to their signals.

## Local Development

### Install Dependencies

```bash
npm ci
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
