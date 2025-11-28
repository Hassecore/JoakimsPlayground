param vnetName string = 'joakimsVNet'
param location string = resourceGroup().location
param publicIpName string = 'joakimsPublicIp'


// Create SQL Server + SQL Db

module joakimsVNet './createVNet.bicep' = {
    name: 'joakimsVnetDeployment'
    params: {
        vnetName: vnetName
        location: location
        publicIpName: publicIpName
    }
}

module joakimsKeyVault './KeyVault/createKeyVault.bicep' = {
    name: 'joakimsKeyVaultDeployment'
    params: {
        keyVaultname: 'joakimsKeyVault3'
        location: location
        subnetId: joakimsVNet.outputs.caSubnetResourceId
    }
}

module joakimsSqlServer './createDatabase.bicep' = {
    name: 'joakimsDbDeployment'
    params: {
        subnetResourceId: joakimsVNet.outputs.subnet1ResourceId
    }
    dependsOn: [
        joakimsVNet
    ]
}

module joakimsApiContainer './createHelloWorldContainer.bicep' = {
    name: 'joakimsApiContainerDeployment'
    params: {
        subnetId: joakimsVNet.outputs.subnet1ResourceId
        location: location
    }
    dependsOn: [
        joakimsVNet
        joakimsSqlServer
    ]
}

module joakimsAuthServerContainer './createAuthServerContainer.bicep' = {
    name: 'joakimsAuthServerContainerDeployment'
    params: {
        subnetId: joakimsVNet.outputs.subnet1ResourceId
        location: location
    }
    dependsOn: [
        joakimsVNet
        joakimsSqlServer
    ]
}

module joakimsContainerApps './ContainerApps/createContainerAppWithEnv.bicep' = {
    name: 'joakimsContainerAppDeployment'
    params: {
        location: location
        subnetId: joakimsVNet.outputs.caSubnetResourceId
    }
    dependsOn: [
        joakimsVNet
    ]
}

module joakimsApplicationGateway 'createApplicationGateway.bicep' = {
    name: 'joakimsAppGatewayDeployment'
    params:{
        location: location
        subnetId: joakimsVNet.outputs.agSubnetResourceId
        publicIpId: joakimsVNet.outputs.publicIpId
        authContainerIp: joakimsAuthServerContainer.outputs.authContainerIp
        helloWorldContainerFQDN: joakimsContainerApps.outputs.containerAppFqdn
    }
    dependsOn: [
        joakimsVNet
        joakimsAuthServerContainer
        joakimsContainerApps
    ]
}