module "key_vault" {
  source              = "innovationnorway/key-vault/azurerm"
  version             = "1.2.1"
  name                = "${local.prefix}-kv"
  resource_group_name = azurerm_resource_group.main.name

  secrets = [
    {
      name  = "Storage--ApiKey"
      value = azurerm_storage_account.main.primary_access_key
    },
    {
      name  = "Storage--ConnectionString"
      value = azurerm_storage_account.main.primary_connection_string
    },
    {
      name  = "Storage--ConnectionStringBlob"
      value = azurerm_storage_account.main.primary_blob_connection_string
    }
  ]
}

resource "azurerm_key_vault_access_policy" "msi" {
  key_vault_id = module.key_vault.id

  tenant_id = data.azurerm_client_config.current.tenant_id
  object_id = module.web_app.identity.principal_id

  key_permissions = []

  secret_permissions = [
    "get",
    "list",
  ]
}
