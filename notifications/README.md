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
    \"latitude\": \"86.0242116775039\",
    \"longitude\": \"117.428656163838\",
    \"timestamp\": \"2023-09-15T10:26:21.6871353Z\",
    \"validationResult\": {
        \"status\": \"invalid\",
        \"error\": \"This signal is invalid\"
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
