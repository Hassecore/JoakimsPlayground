// Create a User Assigned Managed Identity (UAMI)

param uamiName string = 'joakimsUAMI1'
param location string = resourceGroup().location
param tags object = {}

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: uamiName
  location: location
  tags: tags
}

output identityResourceId string = uami.id
output principalId string = uami.properties.principalId
output clientId string = uami.properties.clientId
