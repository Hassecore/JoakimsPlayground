README


QUESTS (to do):

1 - Create SQL Db.
    - Use bicep file to create SQL Server
    - Use bicep file to create SQL Db
    - Use bicep file to assign firewall rule for the SQL Server

2 - Create Auth Server
    - Create ASP.NET Api using: 
        - Microsoft.AspNetCore.Authentication
        - Microsoft.AspNetCore.Identity.EntityFrameworkCore
    - Set up code first EF Core context + migration(s)
    - Create endpoint to register a user
    - Create endpoint to login (endpoint should return JWT)

3 - Create API with JWT auth
    - Create ASP.NET Api
    - Create some simple endpoints
    - Register auth server JWT authentication
    - Add auth on endpoints
        - No auth needed
        - Needs to be authed
        - Needs to be 'User' role
        - Needs to be 'Admin' role

4 - Dockerize Apps
    - Dockerize the apps 
    - Build image on local
    - Run image(s) on local
    - Push image to docker hub
    - Set up docker compose
    - Add to bicep and deploy to Azure as Container Instances

? - VNet connection (static IP - https://learn.microsoft.com/en-us/azure/container-instances/container-instances-egress-ip-address)
					(Tutorial to create a VNet - https://learn.microsoft.com/en-us/azure/virtual-network/quickstart-create-virtual-network?tabs=portal)
    - Add VNet
    - Add VNet/SubNet connection to container ContainerInstance
    - Add VNet firewall rule to allow VNet ip to access sql server
    - Make sure container has access to db via VNet

? - Split deployment into sub-files
    - Create a main.bicep
    - Create bicep sub-files for
        - Sql Server
        - VNet
        - Container App(s)
        - ?


? - Add KeyVault
	- Add key vault in bicep deployment
	- Store secrets in key vault (e.g. SQL Server password)
	- Retrieve secrets from key vault
	- Grant access to ARM for access during deployment
	- Grant access via user-managed identity
		- Container Apps need to access keyVault


? - Parameterize bicep deployment (Create parameter file to easily input everything needed)

? - Container App Environment
	- Create Container App Environment
	- Deploy each app as Container App within that env
	- With access to Key Vault via User-Assigned Managed Identity

? - Application Gateway
	- Configure app gateway so APIs can be reached on public endpoint




? - Add https support
? - Add MongoDb
    - Create bicep to create/deploy MongoDb
    - Add mongo CRUD in API
? - Add Service Bus
? - Add Az.Functions
    - Service Bus trigger
    - Http trigger
? - Add ElasticSearch
? - Go through OKR document
? - Add OIDC support?
? - Add a React frontend?
? - Use Storage account (Blob storage, Queues, Table storage)
? - 
? -
? - 
? -