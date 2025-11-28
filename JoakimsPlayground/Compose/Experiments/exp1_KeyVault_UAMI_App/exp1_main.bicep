param location string = resourceGroup().location

param nmbr string = '21'

// create UAMI
module joakimsUami './exp1_UAMI.bicep' = {
    name: 'joakimsUamiDeployment'
    params: {
        uamiName: 'joakimsUAMI${nmbr}'
        location: location
    }
}

// create VNET
module joakimsVNet './exp1_CreateVNet.bicep' = {
    name: 'joakimsVnetDeployment'
    params: {
        vnetName: 'joakimsVNet${nmbr}'
        location: location
        //publicIpName: publicIpName
    }
}

//create KV and give UAMI access to KV
module joakimsKeyVault './exp1_CreateKeyVault.bicep' = {
    name: 'joakimsKvDeployment'
    params: {
        keyVaultname: 'joakimsKeyVault${nmbr}'
        location: location
        uamiPrincipalId: joakimsUami.outputs.principalId
        subnetId: joakimsVNet.outputs.kvSubnetResourceId
    }
    dependsOn: [
        joakimsUami
    ]
}

// Reference the existing Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: joakimsKeyVault.outputs.keyVaultName
}


// create DB and set sql PW from KV secret
module joakimsSqlServer './exp1_CreateDatabase.bicep' = {
    name: 'joakimsSqlDeployment'
    params: {
        serverName: 'joakimsSqlServer${nmbr}'
        sqlDbName: 'joakimsSqlDb${nmbr}'
        location: location
        kvName: joakimsKeyVault.outputs.keyVaultName
        // secretValue: joakimsKeyVault.outputs.sqlServerPW
        subnetResourceId: joakimsVNet.outputs.subnet1ResourceId
        sqlServerPW: keyVault.getSecret(joakimsKeyVault.outputs.sqlServerPwSecretName)
    }
    dependsOn: [
        joakimsKeyVault
        joakimsUami
    ]
}

module joakimsAuthServer './exp1_CreateAuthServer.bicep' = {
    name: 'joakimsAuthServerDeployment'
    params: {
        location: location
        uamiId: joakimsUami.outputs.identityResourceId
        nmbr: nmbr
        sqlServerPW: keyVault.getSecret(joakimsKeyVault.outputs.sqlServerPwSecretName) //sqlServerPW
        dockerHubSecret: keyVault.getSecret(joakimsKeyVault.outputs.dockerPwSecretName)
        containerGroupName: 'joakimsAuthContainer${nmbr}'
        subnetId: joakimsVNet.outputs.subnet1ResourceId
    }
    dependsOn: [
        joakimsKeyVault
        joakimsUami
    ]
}

module joakimsCaContainer './exp1_CreateContainerAppWithEnv.bicep' = {
    name: 'joakimsCaContainerDeployment'
    params: {
        location: location
        uamiPrincipalId: joakimsUami.outputs.identityResourceId
        subnetId: joakimsVNet.outputs.caSubnetResourceId
        containerAppName: 'joakims-container${nmbr}'
        containerAppEnvName: 'joakimsContainerAppEnv${nmbr}'
        logWorkspaceName:'joakimsLogWorkspaceName${nmbr}'
        appInsightsName: 'joakimsAppInsights${nmbr}'
        keyVaultName: joakimsKeyVault.outputs.keyVaultName
        dockerPwSecretName: joakimsKeyVault.outputs.dockerPwSecretName
        dockerHubSecret: keyVault.getSecret(joakimsKeyVault.outputs.dockerPwSecretName)
    }
    dependsOn: [
        //joakimsKeyVault
        joakimsUami
    ]
}






// check that migration was applied to DB.
