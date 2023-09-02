// move to mid
resource "google_cloud_run_v2_service" "aggregation-application" {
  name     = "aggregation-application"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/aggregation-application:${var.AGGREGATION_VERSION}"
      env {
        name  = "aggregation_signals"
        value = google_pubsub_topic.aggregation-signals.id
      }
    }

    scaling {
      // it's usefull to limit processing and spot issues via subscriptions, 
      // instead of a high instance count
      max_instance_count = 5
    }
    labels = {
      bounded-context = "aggregation"
      component       = "core"
    }
  }
}

resource "google_cloud_scheduler_job" "aggregation-scheduler" {
  name        = "aggregation-schedule"
  description = "Aggregation Scheduler"
  schedule    = "*/10 * * * *"

  retry_config {
    retry_count = 1
  }

  http_target {
    http_method = "POST"
    uri         = "${google_cloud_run_v2_service.aggregation-application.uri}/aggregate-from-providers"
    oidc_token {
      // in early/mid we use hard coded values
      # service_account_email = "642254565385-compute@developer.gserviceaccount.com"
      service_account_email = data.google_compute_default_service_account.default.email
      audience              = google_cloud_run_v2_service.aggregation-application.uri
    }
  }
}

resource "google_pubsub_topic" "aggregation-signals" {
  name = "aggregation-signals"
  labels = {
    bounded-context = "aggregation"
    component       = "core"
  }
}
