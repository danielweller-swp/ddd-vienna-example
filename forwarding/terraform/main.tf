variable "AGGREGATION_VERSION" {
  type        = string
  description = "GIT-REF"
}


data "google_compute_default_service_account" "default" {
}

data "google_pubsub_topic" "aggregation-signals" {
  name = "aggregation-signals"
}