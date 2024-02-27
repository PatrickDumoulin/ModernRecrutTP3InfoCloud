param appNames array
param location string
@allowed([
  'B1'
  'S1'
  'F1'
])
param sku string = 'F1' 
param spName string = 'sp-modernrecrut'


//Cr�ation du plan de service
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: spName
  location: location
  sku: {
    name: sku
    capacity: 1
  }
}

//Cr�ation de la Web App 
resource webApp 'Microsoft.Web/sites@2022-09-01' = [for appName in appNames : {
  name: 'webapp-${appName}-${uniqueString(resourceGroup().id)}'
  location: location
  tags: {
    Application: appName
  }
  properties: {
    serverFarmId: appServicePlan.id
  }
}
]
