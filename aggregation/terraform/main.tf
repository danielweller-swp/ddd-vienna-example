variable "AGGREGATION_VERSION" {
  type        = string
  description = "GIT-REF"
}


data "google_compute_default_service_account" "default" {
}

