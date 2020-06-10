variable "azure_location" {
  default     = "westeurope"
  description = "The location where the resources should be created."
}

variable "naming_prefix" {
  default     = "bouvet-"
  description = "The naming prefix to use on the created resources"
}

variable "naming_domain" {
  default     = "simple-test"
  description = "The naming prefix to use on the created resources"
}

variable "environment" {
  description = "The environment where the infrastructure is deployed"
  default     = "dev"
}

variable "release" {
  description = "The release the deploy is based on"
  default     = "0.0.1"
}

variable "tags" {
  description = "A map of tags to add to all resources"
  type        = "map"

  default = {}
}

variable "failover_location" {
  description = "Where the CosmosDB fail-over location resides"

  default = "northeurope"
}

locals {

  prefix_api = "${var.naming_prefix}${var.naming_domain}-api-${var.environment}"

  prefix            = "${var.naming_prefix}${var.naming_domain}-${var.environment}"
  name_function_app = "${var.naming_prefix}${var.naming_domain}"
  prefix_strip      = "${replace(local.prefix, "-", "")}"


  name_web_app_full = "${local.prefix_api}-web"
  hostname          = "${local.name_web_app_full}.azurewebsites.net"
  full_hostname     = "https://${local.hostname}/"
  auth_hostname     = "${local.full_hostname}.auth/login/aad/callback"


  tags = merge({
    environment = var.environment
    release     = var.release
  }, var.tags)

}