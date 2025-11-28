param nmbr string = '21'

param topicName string = 'joakimsEventGridTopic${nmbr}'

param location string = resourceGroup().location

@allowed([
  'Basic'
  'Premium'
])
param sku string = 'Basic'

param identityType string = 'None'

// Retrieve function app keys
param functionAppName string = 'joakimsFunctionApp${nmbr}'

resource functionApp 'Microsoft.Web/sites@2024-11-01' existing = {
  name: functionAppName
}

var hostKeys = listKeys('${functionApp.id}/host/default', '2023-12-01')

var functionAppDefaultHostKey = hostKeys.functionKeys.default
// Retrieved function app key for subscriptions

// Retrieve service bus SAS key:
resource sbNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: 'joakimsServerBusNamespace${nmbr}'
}

var serviceBusKey = listKeys('${sbNamespace.id}/AuthorizationRules/joakimsSharedAccessPolicy${nmbr}', '2022-10-01-preview').primaryKey
// retrieved service bus SAS key

resource eventGridTopic 'Microsoft.EventGrid/topics@2023-12-15-preview' = {
	name: topicName
	location: location
	sku: {
		name: sku
	}
	properties: {
		publicNetworkAccess: 'Enabled'
	}
	identity: identityType == 'None' ? null : {
		type: identityType
	}
}


resource webhookSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2023-12-15-preview' = {
  name: '${eventGridTopic.name}/toFunctionApp1'
  properties: {
	destination: {
	  endpointType: 'WebHook'
	  properties: {
		endpointUrl: 'https://${functionAppName}.azurewebsites.net/api/Function1?code=${functionAppDefaultHostKey}'
	  }
	}
	filter: {
	  isSubjectCaseSensitive: false
	  subjectBeginsWith: ''
	  subjectEndsWith: ''
	  includedEventTypes: null // All event types
	  advancedFilters: []
	}
	retryPolicy: {
	  maxDeliveryAttempts: 30
	  eventTimeToLiveInMinutes: 1440 // 24 hours
	  retryDelayInSeconds: 10
	  maxRetryDelayInSeconds: 60
	}
	// deadLetterDestination: null
	// labels: []
	// provisioningState: 'Succeeded'
  }
  dependsOn: [
	eventGridTopic
  ]
}

resource webhookSubscription3 'Microsoft.EventGrid/topics/eventSubscriptions@2023-12-15-preview' = {
  name: '${eventGridTopic.name}/toFunctionApp3'
  properties: {
	destination: {
	  endpointType: 'WebHook'
	  properties: {
		endpointUrl: 'https://${functionAppName}.azurewebsites.net/api/HttpTrigger3?code=${functionAppDefaultHostKey}'
	  }
	}
	filter: {
	  isSubjectCaseSensitive: false
	  subjectBeginsWith: ''
	  subjectEndsWith: ''
	  includedEventTypes: null // All event types
	  advancedFilters: []
	}
	retryPolicy: {
	  maxDeliveryAttempts: 30
	  eventTimeToLiveInMinutes: 1440 // 24 hours
	  retryDelayInSeconds: 10
	  maxRetryDelayInSeconds: 60
	}
	// deadLetterDestination: null
	// labels: []
	// provisioningState: 'Succeeded'
  }
  dependsOn: [
	eventGridTopic
  ]
}

resource serviceBusQueueSubscription 'Microsoft.EventGrid/topics/eventSubscriptions@2023-12-15-preview' = {
  name: '${eventGridTopic.name}/toServiceBusQueue1'
  properties: {
	  destination: {
		  endpointType: 'ServiceBusQueue'
		  properties: {
			  resourceId: '/subscriptions/49e876e6-7be3-48ad-a1ed-37146efe8712/resourceGroups/rg-sandbox-dev-we/providers/Microsoft.ServiceBus/namespaces/joakimsServerBusNamespace21/queues/joakimsqueuename${nmbr}'
			  queueName: 'joakimsqueuename${nmbr}'
			  authenticationType: 'Sas'
			  sasKey: '3K/Ypid+AH7hon2RjFRt7fFfbFk7Ekijr+ASbAIi6B0='
			  sasKeyName: 'joakimsSharedAccessPolicy${nmbr}'
		  }
	  }
	  filter: {
		  isSubjectCaseSensitive: false
		  subjectBeginsWith: ''
		  subjectEndsWith: ''
	  }
	  retryPolicy: {
		  maxDeliveryAttempts: 30
		  eventTimeToLiveInMinutes: 1440 // 24 hours
	  }
  }
  dependsOn: [
	eventGridTopic
  ]
}


















output topicEndpoint string = eventGridTopic.properties.endpoint

output topicResourceId string = eventGridTopic.id