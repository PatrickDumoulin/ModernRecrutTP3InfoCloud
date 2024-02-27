param location string
param sku string = 'F1'

var appNames =[
   'mvc'
   'postulation'
   'emplois'
   'documents'
   'favoris'
]


module AppServices 'Modules/appService.bicep' =  {
  name: 'appService'
  params: {
    appNames : appNames
    location: location
    sku: sku
  }
}
