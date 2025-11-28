// To do: Add sharedAccessPolicy to KeyVault

param nmbr string = '21'

param serviceBusNamespaceName string = 'joakimsServerBusNamespace${nmbr}'

// param queueName string = 'joakimsQueueName${nmbr}'

param location string = resourceGroup().location

param sku string = 'Standard'

param capacity int = 0

param policyName string = 'joakimsSharedAccessPolicy${nmbr}'

param keyVaultName string

param sharedAccessPolicySecretName string = 'sbPolicySecret${nmbr}'

param queueName string = 'joakims-queue-name'

// param rights array = [
//   'Listen'
//   'Send'
// ]

// Service Bus Namespace
resource sbNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
	name: serviceBusNamespaceName
	location: location
	sku: {
		name: sku
		tier: sku
		capacity: capacity
	}
	properties: {
		zoneRedundant: sku == 'Premium' // Enables zone redundancy in Premium SKU
	}
}

// added
resource sharedAccessPolicy 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2022-10-01-preview' = {
  name: '${sbNamespace.name}/${policyName}'
  properties: {
    rights: [
		'Manage'
		'Listen'
		'Send'
	]
  }
  dependsOn: [
    sbNamespace
  ]
}

resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
    parent: existingKeyVault
    name: sharedAccessPolicySecretName
    properties: {
        value: listKeys(sharedAccessPolicy.id, '2022-10-01-preview').primaryConnectionString
        attributes: {
            enabled: true
        }
    }
}


// end of added

// Queue in SB Namespace
resource sbQueue 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = {
	parent: sbNamespace
	name: queueName //xxx${queueName}'
	properties: {
		enablePartitioning: true
		requiresDuplicateDetection: false
		maxDeliveryCount: 10
		lockDuration: 'PT1M'
		deadLetteringOnMessageExpiration: true
		defaultMessagetimeToLive: 'P14D' // 14 days
	}
}

output namespaceName string = sbNamespace.name
// output queueName string = sbQueue.name
// output serviceBusFQDN string = sbNamespace.properties.serviceBusEndpoint







