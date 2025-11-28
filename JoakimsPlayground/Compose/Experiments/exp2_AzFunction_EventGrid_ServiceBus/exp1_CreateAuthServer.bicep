param subnetId string
param uamiId string
param nmbr string

@secure()
param sqlServerPW string

@secure()
param dockerHubSecret string

@description('Name of the container group')
param containerGroupName string = 'joakimsAuthContainer'

@description('Docker image')
param image string = 'hassecore/rolebasedauthserver'

@description('Azure region')
param location string = resourceGroup().location

@description('API port to expose')
param authServerPort int = 80

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
    name: containerGroupName
    location: location
    identity: {
        type: 'UserAssigned'
        userAssignedIdentities: {
            '${uamiId}': {}
        }
    }
    properties: {
        imageRegistryCredentials: [
            {
                server: 'index.docker.io'
                username: 'hassecore'
                password: dockerHubSecret
            }
        ]
        ipAddress: {
            type: 'Private'
            ports: [
                {
                    protocol: 'TCP'
                    port: authServerPort
                }
            ]
        }
        osType: 'Linux'
        containers: [
            {
                name: 'auth-server'
                properties: {
                    image: image
                    environmentVariables: [
                        {
                        name: 'JWTKey__ValidAudience'
                        value: 'http://api'
                        }
                        {
                        name: 'JWTKey__ValidIssuer'
                        value: 'http://auth_server'
                        }
                        {
                        name: 'JWTKey__TokenExpiryTimeInHours'
                        value: '3'
                        }
                        {
                        name: 'JWTKey__Secret'
                        value: 'afsdkjasjflxswafsdklk434orqiwup3457u-34oewir4irroqwiffv48mfs'
                        }
                        {
                        name: 'ConnectionStrings__AppDb'
                        value: 'Server=tcp:joakimssqlserver${nmbr}.database.windows.net,1433;Initial Catalog=joakimsSqlDb${nmbr};Persist Security Info=False;User ID=TestUser;Password=${sqlServerPW};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
                        }
                        {
                        name: 'JWTKey_ValidAudience'
                        value: 'http://api'
                        }
                        {
                        name: 'JWTKey_ValidIssuer'
                        value: 'http://auth_server'
                        }
                        {
                        name: 'JWTKey_TokenExpiryTimeInHours'
                        value: '3'
                        }
                        {
                        name: 'JWTKey_Secret'
                        value: 'afsdkjasjflxswafsdklk434orqiwup3457u-34oewir4irroqwiffv48mfs'
                        }
                        {
                        name: 'ConnectionStrings_AppDb'
                        value: 'Server=tcp:joakimstestsqlserver${nmbr}.database.windows.net,1433;Initial Catalog=joakimTestDb${nmbr};Persist Security Info=False;User ID=TestUser;Password=${sqlServerPW};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
                        }
                    ]
                    resources: {
                        requests: {
                            cpu: 1
                            memoryInGB: 1
                        }
                    }
                    ports: [
                        {
                            port: authServerPort
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

//output authContainerIp string = containerGroup.properties.ipAddress.ip