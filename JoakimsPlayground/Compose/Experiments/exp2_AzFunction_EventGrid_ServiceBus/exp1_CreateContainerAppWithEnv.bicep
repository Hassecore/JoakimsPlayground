/// Create containers


param subnetId string
param uamiPrincipalId string

param keyVaultName string
param dockerPwSecretName string

param logWorkspaceName string = 'joakimsLogWorkspaceName1'

param location string = resourceGroup().location
param appInsightsName string = 'joakimsAppInsights1'
param containerAppEnvName string = 'joakimsContainerAppEnv1'

param containerAppName string = 'joakims-container1'

@secure()
param dockerHubSecret string

// param dockerHubSecret string = 'dckr_pat_YejyB9dnal8xvxZrLnYsPTgd2bs'



// Reference the existing Key Vault
// resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
//   name: keyVaultName
// }

// Get the secret from the existing Key Vault
// resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' existing = {
//   parent: keyVault
//   name: dockerPwSecretName
// }


// var secretValue string = secretValue2.value

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
    name: logWorkspaceName
    location: location
    properties: {
        sku: {
            name: 'PerGB2018'
        }
    }
}

resource appInsights 'microsoft.insights/components@2020-02-02' = {
    name: appInsightsName
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
    }
    dependsOn: [
        logAnalytics
    ]
}

resource env 'Microsoft.App/managedEnvironments@2023-08-01-preview' = {
    name: containerAppEnvName
    location: location
    properties: {
        vnetConfiguration: {
            infrastructureSubnetId: subnetId
        }
        appLogsConfiguration: {
            destination: 'log-analytics'
            logAnalyticsConfiguration: {
                customerId: logAnalytics.properties.customerId
                sharedKey: logAnalytics.listKeys().primarySharedKey
            }
        }
    }
    dependsOn: [
        appInsights
    ]
}

resource containerApp 'Microsoft.App/containerApps@2023-08-01-preview' = {
    name: containerAppName
    location: location
    identity: {
        type: 'UserAssigned'
        userAssignedIdentities: {
            '${uamiPrincipalId}': {}
        }
    }
    properties: {
        managedEnvironmentId: env.id
        configuration: {
            registries: [
                {
                    server: 'index.docker.io'
                    username: 'hassecore'
                    passwordSecretRef: 'docker-hub-secret'
                }
            ]
            secrets: [
                {
                    name: 'docker-hub-secret'
                    value: dockerHubSecret //'dckr_pat_YejyB9dnal8xvxZrLnYsPTgd2bs'
                    // keyVaultUrl: 'https://joakimskeyvault18.vault.azure.net/secrets/dockerPwSecret/ffd864c7cb0a4700a2aa12fcad7d6d05'
                    // identity: '/subscriptions/49e876e6-7be3-48ad-a1ed-37146efe8712/resourcegroups/rg-sandbox-dev-we/providers/Microsoft.ManagedIdentity/userAssignedIdentities/joakimsUAMI18'
                }
            ]
            ingress: {
                external: true
                targetPort: 80
                allowInsecure: false
                traffic: [
                    {
                        latestRevision: true
                        weight: 100
                    }
                ]
            }
        }
        template: {
            containers: [
                {
                    name: containerAppName
                    image: 'docker.io/hassecore/api_withauth:latest' //'mcr.microsoft.com/k8se/quickstart:latest'
                    resources: {
                        cpu: json('1.0')
                        memory: '2Gi'
                    }
                }
            ]
            scale: {
                minReplicas: 1
                maxReplicas: 2
            }
        }
    }
    dependsOn: [
        env
    ]
}

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn