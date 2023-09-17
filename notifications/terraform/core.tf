resource "google_cloud_run_v2_service" "notifications-application" {
  name     = "notifications-application"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/notifications_application:${var.NOTIFICATIONS_VERSION}"
    }

    scaling {
      // it's usefull to limit processing and spot issues via subscriptions, 
      // instead of a high instance count
      max_instance_count = 5
    }
    labels = {
      bounded-context = "notifications"
      component       = "core"
    }
  }
}

resource "google_pubsub_subscription" "aggregation-signals-subscription" {
  name  = "notifications-signals"
  topic = "aggregation-signals"

  labels = {
    bounded-context = "notifications"
    component       = "core"
  }

  push_config {
    push_endpoint = google_cloud_run_v2_service.notifications-application.uri
    oidc_token {
      service_account_email = data.google_compute_default_service_account.default.email
    }
  }
}
