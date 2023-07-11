# Collection Function
An Azure Function App that can run in any Azure environment to collect a file from a storage account and upload it to the API.

An example usecase would be to trigger this function as part of a Data Factory pipeline to upload a file result to the API.

It uses an app registration client id and secret for authentication to the API, and uses an Access Key Connection String for storage account connection. 

These are currently stored in environmental variables with an update coming to use a Key Vault.

The variables are as follows:

-  TenantId
-  StorageAccount
-  ServiceUrl
-  ClientId
-  ClientSecret
-  TokenResource


The function will require network connectivity to the desired storage account however no inbound traffic is required to the function from the internet as it is all outbound HTTPS traffic.