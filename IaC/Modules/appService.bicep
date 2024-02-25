param appName string
param location string
@allowed([
  'B1'
  'S1'
  'F1'
])
param sku string = 'F1' 


//Cr�ation du plan de service
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'sp-${appName}'
  location: location
  sku: {
    name: sku
    capacity: 1
  }
  tags: {
    Application: appName
  }
}

//Cr�ation de la Web App 
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: 'webapp-${appName}-${uniqueString(resourceGroup().id)}'
  location: location
  tags: {
    Application: appName
  }
  properties: {
    serverFarmId: appServicePlan.id
  }
}

// Condition pour la création d'un slot staging 
resource stagingSlot 'Microsoft.Web/sites/slots@2022-09-01' = if (sku == 'S1') {
  parent: webApp
  name: '${webApp.name}-staging'
  location: location
  tags: {
    Application: appName
  }
  properties: {
    serverFarmId: appServicePlan.id
  }

}

//Condition pour la création de la règle de mise à l'échelle
resource Autoscale 'Microsoft.Insights/autoscalesettings@2022-10-01' = if (sku == 'S1') {
  name: '${appServicePlan.name}-Autoscale'
  location: location
  tags: {}
  properties: {
    enabled: true
    name: '${appServicePlan.name}-Autoscale'
    targetResourceUri: appServicePlan.id
    profiles: [
      {
        name: 'Auto created default scale condition'
        capacity: {
          minimum: '1'
          maximum: '10'
          default: '1'
        }
        rules: [
          {
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricNamespace: 'microsoft.web/serverfarms'
              metricResourceUri: appServicePlan.id
              operator: 'GreaterThanOrEqual'
              statistic: 'Average'
              threshold: 80
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT10M'
              dimensions: []
              dividePerInstance: false
            }
          }
          {
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricNamespace: 'microsoft.web/serverfarms'
              metricResourceUri: appServicePlan.id
              operator: 'LessThanOrEqual'
              statistic: 'Average'
              threshold: 40
              timeAggregation: 'Average'
              timeGrain: 'PT1M'
              timeWindow: 'PT10M'
              dimensions: []
              dividePerInstance: false
            }
          }
        ]
      }
    ]
    notifications: []
    targetResourceLocation: 'Canada Central'
  }
}


output webAppName string = webApp.name
