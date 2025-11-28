param uamiPrincipalId string
param subnetId string

param keyVaultname string = 'joakimsKeyVault11'
param location string = resourceGroup().location

param secretName string = 'someName1'
param secretValue string = 'someValue1'

param secret2Name string = 'sqlServerPW'
param secret2Value string = '2!@asd_ASoiu123'

param secret3Name string = 'dockerPwSecret'
param secret3Value string = 'dckr_pat_YejyB9dnal8xvxZrLnYsPTgd2bs'

var armAppId = 'aebc5f3a-3d2a-4f4b-abc0-9c5d3e6f41c0'
var armSp = first(listServicePrincipals(armAppId, '2022-07-01').value)

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
    name: keyVaultname
    location: location
    properties: {
        tenantId: subscription().tenantId
        sku: {
            name: 'Standard'
            family: 'A'
        }
        enabledForDeployment: true
        enabledForTemplateDeployment: true
        enabledForDiskEncryption: false
        enableSoftDelete: false
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
            {
                tenantId: subscription().tenantId
                objectId: uamiPrincipalId
                permissions: {
                    secrets: [
                        'get'
                        'list'
                    ]
                }
            }
            // {
            //     tenantId: subscription().tenantId
            //     objectId: 'aebc5f3a-3d2a-4f4b-abc0-9c5d3e6f41c0' // ARM service principal
            //     permissions: {
            //         secrets: [
            //             'get'
            //             'list'
            //         ]
            //     }
            // }
            // {
            //     tenantId: subscription().tenantId
            //     objectId: 'f248a218-1ef9-47bf-9928-ae47093fd442' // ARM service principal
            //     permissions: {
            //         secrets: [
            //             'get'
            //             'list'
            //         ]
            //     }
            // }
            // {
            //     tenantId: subscription().tenantId
            //     objectId: 'f8cdef31-a31e-4b4a-93e4-5f571e91255a' // ARM service principal armSp
            //     permissions: {
            //         secrets: [
            //             'get'
            //             'list'
            //         ]
            //     }
            // }
            // {
            //     tenantId: subscription().tenantId
            //     objectId: armSp.objectId // ARM service principal armSp
            //     permissions: {
            //         secrets: [
            //             'get'
            //             'list'
            //         ]
            //     }
            // }
        ]
        networkAcls: {
            defaultAction: 'Deny' // Deny by defaultAction
            bypass: 'AzureServices' // Allow trusted Az Services
            ipRules: [
               {
                   value: '37.139.141.6'
               }
            ]
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

resource keyVaultSecret3 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
    parent: keyVault
    name: secret3Name
    properties: {
        value: secret3Value
        attributes: {
            enabled: true
        }
    }
}

output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name

output someSecretName string = secretName
output sqlServerPwSecretName string = secret2Name
output dockerPwSecretName string = secret3Name
