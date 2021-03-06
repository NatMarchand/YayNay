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

resource "azurerm_application_insights" "appinsights" {
  name                = "yaynay${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"

  tags                     = {
    environment            = var.environment
  }
}

resource "azurerm_application_insights_api_key" "quickpulseApikey" {
  name                    = "quickpulse-apikey"
  application_insights_id = azurerm_application_insights.appinsights.id
  read_permissions        = ["agentconfig"]
}

resource "azurerm_app_service_plan" "serviceplan" {
  name                     = "yaynay-${var.environment}"
  location                 = azurerm_resource_group.rg.location
  resource_group_name      = azurerm_resource_group.rg.name

  sku {
    tier = "Basic"
    size = "B1"
  }

  tags                     = {
    environment            = var.environment
  }
}

resource "azurerm_app_service" "appservice" {
  name                    = "yaynay-${var.environment}"
  location                = azurerm_resource_group.rg.location
  resource_group_name     = azurerm_resource_group.rg.name
  app_service_plan_id     = azurerm_app_service_plan.serviceplan.id

  client_affinity_enabled = false
  https_only              = true 

  site_config {
    always_on                 = true
    ftps_state                 = "Disabled"
    http2_enabled             = true
    scm_type                  = "VSTSRM"
    use_32_bit_worker_process = false
    websockets_enabled        = true
  }

  logs {
    http_logs {
      file_system {
          retention_in_days = 7
          retention_in_mb   = 35
      }
    }
  }

  app_settings = {
    # Application insights
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = azurerm_application_insights.appinsights.instrumentation_key
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0"
    "APPINSIGHTS_QUICKPULSEAUTHAPIKEY"                = azurerm_application_insights_api_key.quickpulseApikey.api_key
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0"
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~2"
    "DiagnosticServices_EXTENSION_VERSION"            = "~3"
    "InstrumentationEngine_EXTENSION_VERSION"         = "~1"
    "SnapshotDebugger_EXTENSION_VERSION"              = "~1"
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "~1"
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended"

    # Run from zip
    "WEBSITE_RUN_FROM_PACKAGE"                        = "1"
  }

  tags = {
    environment            = var.environment
  }
}

resource "azurerm_application_insights_web_test" "healthcheck" {
  name                    = "Health Check"
  location                = azurerm_resource_group.rg.location
  resource_group_name     = azurerm_resource_group.rg.name
  application_insights_id = azurerm_application_insights.appinsights.id
  kind                    = "ping"
  frequency               = 300
  timeout                 = 120
  enabled                 = true
  retry_enabled           = true
  geo_locations           = ["emea-nl-ams-azr", "us-ca-sjc-azr", "emea-fr-pra-edge", "us-va-ash-azr"]

  configuration = <<XML
<WebTest Name="Health check" Id="9210f124-4e05-4d0f-9b68-bd89009785b7" Enabled="True" CssProjectStructure="" CssIteration="" Timeout="120" WorkItemIds="" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010" Description="" CredentialUserName="" CredentialPassword="" PreAuthenticate="True" Proxy="default" StopOnError="False" RecordedResultFile="" ResultsLocale="">
  <Items>
    <Request Method="GET" Guid="a78f328b-5d76-957f-fe64-f72c316e51e2" Version="1.1" Url="https://${azurerm_app_service.appservice.default_site_hostname}/health" ThinkTime="0" Timeout="120" ParseDependentRequests="False" FollowRedirects="True" RecordResult="True" Cache="False" ResponseTimeGoal="0" Encoding="utf-8" ExpectedHttpStatusCode="200" ExpectedResponseUrl="" ReportingName="" IgnoreHttpStatusCode="False" />
  </Items>
</WebTest>
XML
  tags = {
    environment            = var.environment
  }
}

output "apiapp_name" {
  value = azurerm_app_service.appservice.name
}

output "apiapp_hostname" {
  value = azurerm_app_service.appservice.default_site_hostname
}