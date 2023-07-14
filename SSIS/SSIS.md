# SQL Server Integration Services (SSIS) - Sample Packages

There are two sample packages in thie SSIS example show how to execute a UDB workflow. The first package named **Execute UDB Workflow.dtsx**, is a parameterised package which contains a single script task written in C# tha utilises the System.Net.Http.HTTPClient to post a file along with the require parameters to a workflow.

The second package, **Sample.dtsx**, is an example of how to call the first **Execute UDB Workflow.dtsx** package and how to pass the parameters required to it. The sample package has a list variables that need to be completed. These variables are passed through to the parameters of the Execute UDB Workflow.

The table below describes the parameters which would be required to execute a workflow.

| Parameter | Description |
| ----------- | ----------- |
| ClientId | The user or application ID that is being used to execute the workflow. |
| Client_Secret | The client secret or password related tot he user or application. |
| Resource | The Azure resource identifier. This will usually be **api://8b749164-376f-42b2-be78-199c97c2526c** |
| Url | The URL to the SCW UDB platform. This will usually be **https://rtd.scwcsu.nhs.uk**.|
| WorkflowID | The identity of the workflow within the UDB. |
| FilePath | The full path to the file that needs to be uploaded. |



## **Execution results** ##

### **Success** ###

If the script task within the **Execute UDB Workflow.dtsx** package successfully executes the workflow, it will return ScriptResults.Success as the TaskResult meaning the step will succeed within the control flow. A message such as that shown below will appear in the package process log.

>[UDB Workflow] Information: WorkflowId: 5f0379e9-95f5-434d-9eed-d17dd9ed5a13 was started successfully.
{"executionGuid":"4a3d72fd-3f9c-41d0-a511-fbbfd5d8c5e6","progress":[{"created":"2023-07-13T15:10:57.3816567Z","status":"Continuing","message":"Beginning execution of 'My workflow' with data 'File1.csv'","source":"Workflow Engine"},{"created":"2023-07-13T15:10:57.4238093Z","status":"Continuing","message":"Beginning execution of process step 'AntiVirusScan' with data 'File1.csv'","source":"Workflow Engine"},{"created":"2023-07-13T15:10:57.4543141Z","status":"Continuing","message":"Execution of process step 'AntiVirusScan' has completed successfully","source":"Workflow Engine"},{"created":"2023-07-13T15:10:57.8975093Z","status":"Queued","message":"Execution of step 'Local - Save file' has been queued to be continued on Host 'SCW-RemoteHost' (Event: 'ExecuteRemote')","source":"Workflow Engine"},{"created":"2023-07-13T15:10:57.9256008Z","status":"Queued","message":"Execution of 'My workflow' has partially completed, other steps have been queued for specialized resources","source":"Workflow Engine"}],"message":"Upload Succeeded: Queued","status":200}

The script task does not wait for the workflow to execute and it is not possible from the task in this example to determin whether the workflow was successful. The UDB API does wllow the querying of execusion results, though this wasn't within the scope of this sample.

### **Failure** ###
Should the script task within the **Execute UDB Workflow.dtsx** package fail to execute the workflow, it will return ScriptResults.Failure as the TaskResult failing the current step within the workflow. A message including details of the failure will be written to the progress log.

Something along the lines of:

>[UDB Workflow] Error: UDB workflow execution error: Unabable to get a token: Unauthorized

## Troubleshooting ##
1. Check all of the parameters are correct.
2. Make sure that the ClientId has permissions within the Organisation to execute a workflow.
3. Ensure the organisation being used has access to the workflow.
4. This package will **not** work with SQL Server 2012 and below due to the version of .Net used by SSIS not having the System.Net.Http.HttpClient class.