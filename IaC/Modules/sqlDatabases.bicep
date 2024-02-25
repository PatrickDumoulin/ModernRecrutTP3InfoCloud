param location string 
param serverName string
param dbSettings array
param dbUser string
@minLength(10)
@maxLength(20)
@secure()
param dbPassword string

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  location:location
  name:'srv-${serverName}'
  properties:{
    administratorLogin:dbUser
    administratorLoginPassword:dbPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01'=[for dbSetting in dbSettings :{
  location:location
  name:'db-${dbSetting.Name}'
  parent:sqlServer
  sku:{
    name: dbSetting.sku
    tier: dbSetting.sku
  }
}]

resource AllowAllIps 'Microsoft.Sql/servers/firewallRules@2021-11-01'={
  name:'${serverName}-AutoriserToutIps'
  parent:sqlServer
  properties:{
    startIpAddress:'0.0.0.0'
    endIpAddress:'255.255.255.255'
  }
}
