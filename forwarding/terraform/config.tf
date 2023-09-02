locals {
  GCP_PROJECT = "ddd-vienna-sample"
  location    = "europe-west1"
}

terraform {
  required_version = "~> 1.5.0"

  required_providers {
    google = "~> 4.79.0"
  }

  backend "gcs" {
  }
}

provider "google" {
  project = local.GCP_PROJECT
  region  = "europe-west1"
  zone    = "europe-west1-b"
}
