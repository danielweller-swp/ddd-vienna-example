variable "AGGREGATION_VERSION" {
  type        = string
  description = "GIT-REF"
}

module "pubsub" {
  source = "./pubsub"

  version_ref = var.AGGREGATION_VERSION
}

module "webhook" {
  source = "./webhook"

  version_ref = var.AGGREGATION_VERSION
}
