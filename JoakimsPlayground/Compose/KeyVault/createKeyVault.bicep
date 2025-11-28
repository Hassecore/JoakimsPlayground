param subnetId string
param keyVaultname string = 'joakimsKeyVault3'
param location string = resourceGroup().location

param secretName string = 'someName1'
param secretValue string = 'someValue1'

param secret2Name string = 'sqlServerPW'
param secret2Value string = '2!@asd_ASoiu123'


resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
    name: keyVaultname
    location: location
    properties: {
        tenantId: subscription().tenantId
        sku: {
            name: 'Standard'
            family: 'A'
        }
        accessPolicies: [
            {
                tenantId: subscription().tenantId
                objectId: '26dda872-9dbc-4263-bb60-827ca2a71212'
                permissions: {
                    secrets: [
                        'get'
                        'list'
                    ]
                }
            }
            // Add access policy for container app (System-Assigned User Identity)
        ]
        networkAcls: {
            defaultAction: 'Deny' // Deny by defaultAction
            bypass: 'AzureServices' // Allow trusted Az Services
            //ipRules: [
            //    {
            //        value: '37.139.141.6'
            //    }
            //]
            virtualNetworkRules: [
                {
                    id: subnetId
                    ignoreMissingVnetServiceEndpoint: false
                }
            ]
        }
    }
}

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
    parent: keyVault
    name: secretName
    properties: {
        value: secretValue
        attributes: {
            enabled: true
        }
    }
}

resource keyVaultSecret2 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
    parent: keyVault
    name: secret2Name
    properties: {
        value: secret2Value
        attributes: {
            enabled: true
        }
    }
}