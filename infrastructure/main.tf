data "azurerm_client_config" "current" {}

provider "azurerm" {
  version = ">=1.31.0"
}

provider "azuread" {
  version = ">=0.4.0"
}

resource "azurerm_resource_group" "main" {
  name     = "${local.prefix_api}-rg"
  location = var.azure_location

  tags = local.tags
}

module "web_app" {
  source  = "innovationnorway/web-app/azurerm"
  version = "1.0.0"
  name    = "${local.prefix_api}-web"

  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  https_only          = true

  plan = {
    name     = "${local.prefix_api}-plan"
    os_type  = "windows"
    sku_size = "S1"
  }

  runtime = {
    name    = "node"
    version = "10.6"
  }

  scaling = {
    enabled = true
  }

  app_settings = {
    KeyVault__BaseUri        = module.key_vault.uri
    WEBSITE_RUN_FROM_PACKAGE = "1"


    Logging__RollingFile__Path = "../../LogFiles/log.txt"

    APPINSIGHTS_INSTRUMENTATIONKEY = azurerm_application_insights.main.instrumentation_key
  }

  auth = {
    enabled = true
    active_directory = {
      client_id     = module.application.application_id
      client_secret = module.application.password
      allowed_audiences = [
        local.auth_hostname
      ]
    }
  }

  tags = local.tags
}

resource "azurerm_storage_account" "main" {
  name                = "${local.prefix_strip}sta"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"

  access_tier = "Hot"
}

module "application" {
  source = "innovationnorway/application/azuread"

  name     = local.name_web_app_full
  homepage = local.full_hostname

  identifier_uris = [local.full_hostname]
  reply_urls      = [local.auth_hostname]

  api_permissions = [
    {
      name = "Microsoft Graph"
      oauth2_permissions = [
        "User.Read"
      ]
    }
  ]

  app_roles = [
    {
      description  = "Admins can manage roles and perform all task actions"
      name         = "Admin"
      member_types = ["User", "Application"]
      enabled      = true
      value        = "Admin"
    }
  ]
}

resource "azuread_service_principal" "main" {
  application_id = module.application.application_id
}

resource "azurerm_application_insights" "main" {
  name                = "${local.prefix}-appinsights"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  application_type    = "web"
}

resource "random_password" "seq_web_app" {
  length  = 32
  special = false
}

resource "random_password" "seq_function" {
  length  = 32
  special = false
}

module "service_bus" {
  source  = "innovationnorway/service-bus/azurerm"
  version = "1.0.0"

  name                = "${local.prefix}-sbs"
  resource_group_name = azurerm_resource_group.main.name

  queues = [
    {
      name = "q-form-submitted"
      authorization_rules = [
        {
          name   = "FunctionListenPolicy"
          rights = ["listen"]
        }
      ]
    }
  ]
}
