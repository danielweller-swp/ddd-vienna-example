#terraform import google_cloud_run_v2_service.pubsub-application europe-west1/forwarding-pubsub
#terraform import google_pubsub_subscription.pubsub-subscription forwarding-pubsub-signals

#terraform import google_cloud_run_v2_service.webhook-application europe-west1/forwarding-webhook
#terraform import google_pubsub_subscription.webhook-subscription forwarding-webhook-signals