resource "google_cloud_run_v2_service" "pubsub-application" {
  name     = "forwarding-pubsub"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/pubsub-application:${var.AGGREGATION_VERSION}"
    }

    labels = {
      bounded-context = "forwarding"
      component       = "pubsub"
    }
  }
}

resource "google_pubsub_subscription" "pubsub-subscription" {
  name  = "forwarding-pubsub-signals"
  topic = data.google_pubsub_topic.aggregation-signals.name

  labels = {
    bounded-context = "forwarding"
    component       = "pubsub"
  }

  push_config {
    push_endpoint = google_cloud_run_v2_service.pubsub-application.uri
    oidc_token {
      service_account_email = data.google_compute_default_service_account.default.email
    }
  }
}
