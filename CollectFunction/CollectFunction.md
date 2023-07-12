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

When calling the function a HTTP Post should be triggered with a body containing the follow:
| Name | Purpose |
| ---- | ------- |
| Container | Storage account container to be accessed |
| Path | Path with container for any folder structure |
| FileName | Name and extension of the file to be accessed | 
| WorkflowGuid | Id of the UDB workflow to be executed |
| OrganisationGuid | Id of the organisation workflow is being executed as | 

For example:

```javascript
{
    "Container": "outgoing",
    "Path": "FileOutputs/ToUpload",
    "FileName": "test.csv",
    "WorkflowGuid": "a002c63e-964b-4582-8774-668178b84b14",
    "OrganisationGuid": "5119d21c-d90a-4dea-a02b-158a547e6bc3"
}
```