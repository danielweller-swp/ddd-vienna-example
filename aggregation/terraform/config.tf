locals {
  GCP_PROJECT = "ddd-vienna-sample"
}

terraform {
  required_version = "~> 1.3.0"

  required_providers {
    google = "~> 4.19.0"
  }

  backend "gcs" {
  }
}

provider "google" {
  project = local.GCP_PROJECT
  region  = "europe-west1"
  zone    = "europe-west1-b"
}
