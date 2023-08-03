# Forwarding

The purpose of the **forwarding** BC is to forward 
signals received from the aggregation BC to customers.

## Deployables

The bounded context contains two deployables:

- `Application.PubSubForwarding`: Forward signals to customers via Pub/Sub.
- `Application.WebhookForwarding`: Forward signals to customers via Webhooks.
