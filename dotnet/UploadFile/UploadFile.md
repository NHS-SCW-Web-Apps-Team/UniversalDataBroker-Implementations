#Execute Class - Real Time Data Platform

##Execution
Call the Execute class with the following parameters.
###Parameter setup
|Parameter	|Description	|Example |
|-----------|:--------------|:------------|
|WorkflowId	|A unique Identifier for the Workflow	| |
|FilePath	|The path to the file being uploaded	| |
|OrganisationId	|A unique Identifier for the organisation	| |
|Url	|The URL of the Site	| |
|ClientId	|A public identifier for apps	| |
|Client_Secret	|A password for the server	| |
|Resourse	|Gives access to parts of the server	| |


###Example

namespace Nhs.Scwcsu.RealTimeData.RTDPTest
{
    public static class Test
    {
        
        public static async Task Main()
        {
            string WorkflowID = ";
            string FilePath = "";
            string OrganisationID = "";
            string Url = "";
            string ClientId = "";
            string Client_Secret = "";
            string Resource = "";

            await ExecuteWorkflow.Execute(WorkflowID, FilePath, OrganisationID, Url, ClientId, Client_Secret, Resource);
        }
    }
}
