// move to mid
resource "google_cloud_run_v2_service" "aggregation-application" {
  name     = "aggregation-application"
  location = local.location
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    containers {
      image = "eu.gcr.io/${local.GCP_PROJECT}/aggregation_application:${var.AGGREGATION_VERSION}"
    }

    labels = {
      bounded-context = "aggregation"
      component       = "core"
    }
  }

}

// in early/mid we use hard coded values
data "google_compute_default_service_account" "default" {
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