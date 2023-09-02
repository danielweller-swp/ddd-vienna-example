// move to mid
resource "google_cloud_run_v2_service" "webhook-application" {
  name     = "forwarding-webhook"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/webhook-application:${var.AGGREGATION_VERSION}"
    }

    labels = {
      bounded-context = "forwarding"
      component       = "webhook"
    }
  }
}


resource "google_pubsub_subscription" "webhook-subscription" {
  name  = "forwarding-webhook-signals"
  topic = data.google_pubsub_topic.aggregation-signals.name

  labels = {
    bounded-context = "forwarding"
    component       = "webhook"
  }

  push_config {
    push_endpoint = google_cloud_run_v2_service.webhook-application.uri
    oidc_token {
      service_account_email = data.google_compute_default_service_account.default.email
    }
  }
}
