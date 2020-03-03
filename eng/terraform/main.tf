provider "azurerm" {
  version = "~>2.0.0"
  features {}
}

terraform {
  backend "azurerm" {}
}

variable "environment" {
  description = "Name of the environment"
}

variable "location" {
  description = "Azure location to use"
}

resource "azurerm_resource_group" "rg" {
    name                = "yaynay-${var.environment}"
    location            = var.location
    tags                = {
      environment       = var.environment
    }
}

resource "azurerm_storage_account" "storage" {
    name                     = "yaynay${var.environment}"
    location                 = azurerm_resource_group.rg.location
    resource_group_name      = azurerm_resource_group.rg.name
    account_tier             = "Standard"
    account_replication_type = "LRS"
    static_website {
      index_document         = "index.html"
      error_404_document     = "index.html"
    }
    tags                     = {
      environment            = var.environment
    }
}

output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "location" {
  value = var.location
}

output "environment" {
  value = var.environment
}