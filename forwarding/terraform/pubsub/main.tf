variable "version_ref" {
  type        = string
  description = "GIT-REF"
}
locals {
  GCP_PROJECT = "ddd-vienna-sample"
  location    = "europe-west1"
}

data "google_compute_default_service_account" "default" {
}

data "google_pubsub_topic" "aggregation-signals" {
  name = "aggregation-signals"
}