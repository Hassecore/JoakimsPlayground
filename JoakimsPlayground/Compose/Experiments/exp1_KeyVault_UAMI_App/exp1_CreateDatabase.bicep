param subnetResourceId string

@description('The name of the sql server')
param serverName string = 'joakimsSqlServer2'

@description('The name of the sql db')
param sqlDbName string = 'joakimTestDb2'

@description('The name of the sql user')
param sqlUser string = 'TestUser'

// @description('The pw of the sql user')
// param sqlPW string = '2!@asd_ASoiu123'

param location string = resourceGroup().location

param kvName string
// param secretName string = 'sqlServerPW'

resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: kvName
}

@secure()
param sqlServerPW string


resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: serverName
  location: location
  tags: {
    tagName1: 'tagValue1'
    tagName2: 'tagValue2'
  }
  properties: {
    administratorLogin: sqlUser
    administratorLoginPassword: sqlServerPW //'@Microsoft.KeyVault(SecretUri=${kv.properties.vaultUri}secrets/${secretName}/)' // sqlPW
    
    publicNetworkAccess: 'Enabled'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource firewallRuleAllowJoakimsIp 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  name: 'AllowJoakimsIp'
  parent: sqlServer
  properties: {
    endIpAddress: '37.139.141.6'
    startIpAddress: '37.139.141.6'
  }
}

resource sqlSubnetRule 'Microsoft.Sql/servers/virtualNetworkRules@2022-05-01-preview' = {
  name: '${sqlServer.name}/AllowSubnetAccess'
  properties: {
    virtualNetworkSubnetId: subnetResourceId
    ignoreMissingVnetServiceEndpoint: false
  }
  dependsOn: [
    sqlServer
  ]
}