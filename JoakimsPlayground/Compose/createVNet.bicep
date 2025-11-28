//createVNET

param vnetName string = 'joakimsVnet'
param location string = resourceGroup().location
param publicIpName string = 'joakimsPublicIp'

var agSubnetName = 'agSubnet'
var agSubnetPrefix = '10.137.0.0/24'

var subnet1Name = 'subnet1'
var subnet1Prefix = '10.137.1.0/24'

var subnet2Name = 'subnet2'
var subnet2Prefix = '10.137.2.0/24'

var subnet3Name = 'containerAppSubnet'
var subnet3Prefix = '10.137.4.0/22'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-05-01' = {
    name: vnetName
    location: location
    properties: {
        addressSpace: {
            addressPrefixes: [
                '10.137.0.0/16'
            ]
        }
        subnets: [
            {
                name: agSubnetName
                properties: {
                    addressPrefix: agSubnetPrefix
                    delegations: [
                        {
                            name: 'aciDelegation'
                            properties: {
                                serviceName: 'Microsoft.Network/applicationGateways'
                            }
                        }
                    ]
                }
            }
            {
                name: subnet1Name
                properties: {
                    addressPrefix: subnet1Prefix
                    serviceEndpoints: [
                        {
                            service: 'Microsoft.Sql'
                        }
                    ]
                    delegations: [
                        {
                            name: 'aciDelegation'
                            properties: {
                                serviceName: 'Microsoft.ContainerInstance/containerGroups'
                            }
                        }
                    ]
                }
            }
            {
                name: subnet2Name
                properties: {
                    addressPrefix: subnet2Prefix
                }
            }
            {
                name: subnet3Name
                properties: {
                    addressPrefix: subnet3Prefix
                    serviceEndpoints: [
                        {
                            service: 'Microsoft.KeyVault'
                        }
                    ]
                }
            }
        ]
    }
    
}

resource publicIp 'Microsoft.Network/publicIPAddresses@2023-05-01' = {
    name: publicIpName
    location: location
    sku: {
        name: 'Standard'
    }
    properties: {
        publicIPAllocationMethod: 'Static'
        idleTimeoutInMinutes: 4
    }
}


output publicIpId string = publicIp.id

output agSubnetResourceId string = virtualNetwork.properties.subnets[0].id
output subnet1ResourceId string = virtualNetwork.properties.subnets[1].id
output subnet2ResourceId string = virtualNetwork.properties.subnets[2].id
output caSubnetResourceId string = virtualNetwork.properties.subnets[3].id