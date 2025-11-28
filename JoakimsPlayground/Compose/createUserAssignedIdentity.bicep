param location string = resourceGroup().location
param identityName string = 'joakimsUserAssignedIdentity'
param sqlServerName string = 'joakimsTestSqlServer'
param raName string = 'someNameFromJoakim'

@description('The name of the sql server')
param serverName string = 'joakimsTestSqlServer'

@description('The name of the sql db')
param sqlDbName string = 'joakimTestDb1'

@description('The name of the sql user')
param sqlUser string = 'TestUser'

@description('The pw of the sql user')
param sqlPW string = '2!@asd_ASoiu123'

resource joakimsTestSqlServer2 'Microsoft.Sql/servers@2023-05-01-preview' = {
    name: serverName
    location: location
    tags: {
    tagName1: 'tagValue1'
    tagName2: 'tagValue2'
    }
    properties: {
        administratorLogin: sqlUser
        administratorLoginPassword: sqlPW
        publicNetworkAccess: 'Enabled'
    }
}

resource ruleAllowJoakimsIp 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
    name: 'AllowJoakimsIp'
    parent: joakimsTestSqlServer2
    properties: {
        endIpAddress: '37.139.141.6'
        startIpAddress: '37.139.141.6'
    }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
    parent: joakimsTestSqlServer2
    name: sqlDbName
    location: location
    sku: {
        name: 'Standard'
        tier: 'Standard'
    }
}

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
    name: identityName
    location: location
}