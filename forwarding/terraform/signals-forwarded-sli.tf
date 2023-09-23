resource "google_monitoring_custom_service" "forwarding" {
  service_id   = "forwarding"
  display_name = "Forwarding"
}

resource "google_logging_metric" "signals-forwarded-good-sli" {
  name   = "monitoring-signals-forwarded-good-sli"
  filter = <<EOT
    resource.type="cloud_run_revision" AND
    resource.labels.service_name="forwarding-pubsub" OR resource.labels.service_name="forwarding-webhook" AND
    ("Forwarded signal via Webhook" OR "Forwarded signal via Pub/Sub" OR "Batched signal")
  EOT
  metric_descriptor {
    metric_kind  = "DELTA"
    value_type   = "INT64"
    display_name = "A signal was forwarded or batched for forwarding"
  }
}

resource "google_logging_metric" "signals-forwarded-bad-sli" {
  name   = "monitoring-signals-forwarded-bad-sli"
  filter = <<EOT
    resource.type="cloud_run_revision" AND
    resource.labels.service_name="forwarding-pubsub" OR resource.labels.service_name="forwarding-webhook" AND
    ("Error forwarding signal via Webhook" OR "Error forwarding signal via Pub/Sub" OR "Error batching signal")
  EOT
  metric_descriptor {
    metric_kind  = "DELTA"
    value_type   = "INT64"
    display_name = "A signal was not forwarded or batched for forwarding due to an error"
  }
}

resource "google_monitoring_slo" "signals-forwarded" {
  service = google_monitoring_custom_service.forwarding.service_id

  slo_id       = "signals-forwarded"
  display_name = "Signals Forwarded"

  goal                = 0.999
  rolling_period_days = 14

  windows_based_sli {
    window_period = "600s"

    good_total_ratio_threshold {
      threshold = 0.999
      performance {
        good_total_ratio {
          good_service_filter = "metric.type=\"logging.googleapis.com/user/monitoring-signals-forwarded-good-sli\" resource.type=\"cloud_run_revision\""
          bad_service_filter  = "metric.type=\"logging.googleapis.com/user/monitoring-signals-forwarded-bad-sli\" resource.type=\"cloud_run_revision\""
        }
      }
    }
  }
}