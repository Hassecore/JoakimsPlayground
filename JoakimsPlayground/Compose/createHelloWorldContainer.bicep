param location string
param subnetId string

var containerGroupName = 'joakimsApiContainer'
var apiPort = 80
var imageName = 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
    name: containerGroupName
    location: location
    properties: {
        ipAddress: {
            type: 'Private'
            ports: [
                {
                    protocol: 'TCP'
                    port: apiPort
                }
            ]
        }
        osType: 'Linux'
        containers: [
            {
                name: 'helloworldcontainer'
                properties: {
                    image: imageName
                    resources: {
                        requests: {
                            cpu: 1
                            memoryInGB: 1
                        }
                    }
                    ports: [
                        {
                            port: apiPort
                        }
                    ]
                }
            }
        ]
        subnetIds: [
            {
                id: subnetId
            }
        ]
    }
}


// TO DO: assign static private endpoint
// TO DO: Create public ip (Or maybe just need one app gateway for all services)
// TO DO: create Application gateway (Or maybe just need one app gateway for all services)

// https://learn.microsoft.com/en-us/azure/container-instances/container-instances-application-gateway

