/// Create containers
param subnetId string

param logWorkspaceName string = 'joakimsLogWorkspaceName1'

param location string = resourceGroup().location
param appInsightsName string = 'joakimsAppInsights1'
param containerAppEnvName string = 'joakimsContainerAppEnv1'

var containerAppName = 'joakims-container1'


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
        type: 'SystemAssigned'
    }
    properties: {
        managedEnvironmentId: env.id
        configuration: {
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
                    image: 'mcr.microsoft.com/k8se/quickstart:latest'
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