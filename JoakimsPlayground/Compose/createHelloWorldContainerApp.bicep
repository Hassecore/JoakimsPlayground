param location string
param subnetId string
param appName string

var containerGroupName = 'joakimsApiContainer'
var apiPort = 80
var imageName = 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'

resource joakimsCaEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: envName
  location: location
}

resource joakimsContainerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  properties: {
    managedEnvironmentId: joakimsCaEnvironment.id
    configuration: {
      ingress: ingressEnabled ? {
        external: true
        targetPort: apiPort
      } : null
    }
    template: {
      containers: [
        {
          name: appName
          image: 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
          resources: {
            cpu: 1.0
            memory: 1.0
          }
        }
      ]
    }
  }
}