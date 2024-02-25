
param location string 
param storageAccountName string
param containerName string 
param sku string = 'Standard_ZRS'


resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' ={
  kind: 'StorageV2'
  location:location
  name: storageAccountName
  sku:{
    name:sku  
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  parent: storageAccount
  name: 'default'
}


resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: containerName
  parent: blobService
}
