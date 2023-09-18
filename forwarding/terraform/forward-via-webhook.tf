variable "CUSTOMER_1_WEBHOOK_KEY" {
  type = string
}

resource "google_cloud_run_v2_service" "webhook-application" {
  name     = "forwarding-webhook"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/webhook-application:${var.AGGREGATION_VERSION}"
      env {
        name  = "CUSTOMER_1_WEBHOOK_KEY"
        value = var.CUSTOMER_1_WEBHOOK_KEY
      }
    }

    labels = {
      bounded-context = "forwarding"
      component       = "webhook"
    }
  }
}


resource "google_pubsub_subscription" "webhook-subscription" {
  name  = "forwarding-webhook-signals"
  topic = "aggregation-signals"

  labels = {
    bounded-context = "forwarding"
    component       = "webhook"
  }

  push_config {
    push_endpoint = google_cloud_run_v2_service.webhook-application.uri
    oidc_token {
      service_account_email = "642254565385-compute@developer.gserviceaccount.com"
    }
  }
}
