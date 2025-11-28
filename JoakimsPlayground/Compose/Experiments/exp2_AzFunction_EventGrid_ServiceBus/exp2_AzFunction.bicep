param subnetId string
// param sqlConnectionString string
// param serviceBusConnectionString string

param nmbr string = '14'

param functionAppName string = 'joakimsFunctionApp${nmbr}'

param appServicePlanName string = 'joakimsAppServicePlan${nmbr}'

param storageAccountName string = 'joakimsstorageaccount${nmbr}'

param sqlDbName string = 'joakimsSqlDb${nmbr}'

param vnetName string = 'joakimsVnet${nmbr}'

param subnetName string = 'joakimsSubnetName${nmbr}'

param queueName string = 'joakims-queue-name'

param location string = resourceGroup().location

@secure()
param sqlServerPW string

param serviceBusNamespaceName string

resource sbNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: serviceBusNamespaceName
}

resource nsAuthRule 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' existing = {
  name: '${serviceBusNamespaceName}/joakimsSharedAccessPolicy${nmbr}'
}

var sasKeys = listKeys(nsAuthRule.id, '2022-10-01-preview')
var serviceBusConnectionString = sasKeys.primaryConnectionString


// Storage account
resource storage 'Microsoft.Storage/storageAccounts@2025-01-01' = {
    name: storageAccountName
    location: location
    sku: {
        name: 'Standard_LRS'
    }
    kind: 'StorageV2'
}

// App service plan
resource plan 'Microsoft.Web/serverfarms@2024-11-01' = {
    name: appServicePlanName
    location: location
    sku: {
        name: 'Y1'
        tier: 'Dynamic'
    }
    properties: {
        reserved: true // Linux
    }
    kind: 'functionapp,linux'
}


var storageKey = listKeys(storage.id, storage.apiVersion).keys[0].value
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storageKey};EndpointSuffix=core.windows.net'


// Function app
resource functionApp 'Microsoft.Web/sites@2024-11-01' = {
    name: functionAppName
    location: location
    kind: 'functionapp,linux'
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        serverFarmId: plan.id
        siteConfig: {
            linuxFxVersion: 'DOTNET-ISOLATED|8.0'
            vnetRouteAllEnabled: true
            vnetSubnetId: subnetId
            appSettings: [
                {
                    name: 'AzureWebJobsStorage'
                    value: storageConnectionString
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~4'
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: 'dotnet-isolated'
                }
                {
                    name: 'FUNCTIONS_INPROC_NET8_ENABLED'
                    value: '1'
                }
                {
                    name: 'ServiceBusConnection'
                    value: serviceBusConnectionString
                }
                {
                    name: 'ConnectionStrings__SqlDbConnectionString'
                    value: 'Server=tcp:joakimssqlserver${nmbr}.database.windows.net,1433;Initial Catalog=joakimTestDb5;Persist Security Info=False;User ID=TestUser;Password=${sqlServerPW};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
                }
                {
                    name: 'Queuename'
                    value: queueName
                }
            ]
        }
        httpsOnly: true
        storageAccount: {
            id: storage.id
        }
    }
}

var hostKeys = listKeys('${functionApp.id}/host/default', '2023-12-01')

output defaultHostKey string = hostKeys.functionKeys.default