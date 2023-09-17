variable "NOTIFICATIONS_VERSION" {
  type        = string
  description = "GIT-REF"
}


// in early/mid we use hard coded values
data "google_compute_default_service_account" "default" {
}

