namespace Implementations.Dotnet.Testing {
    using Dotnet.UploadFile;
    public class UploadFileTests {
        [Fact]
        public async Task Exception_NoWorkflowIdAsync() {
            try {
                string success = await ExecuteWorkflow.Execute("", "filePath", "organisationId", "url", "clientId", "clientSecret", "resource");
            } catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No workflowId provided", ex.Message);
            }
        }
        [Fact]
        public async Task Exception_NoFilePath() {
            try {
                string success = await ExecuteWorkflow.Execute("WorkflowId", "", "organisaitionId", "url", "clientId", "clientSecret", "resource");
            }catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No filepath provided", ex.Message);
            }
        }
        [Fact]
        public async Task Exception_NoOrganisationId() {
            try {
                string success = await ExecuteWorkflow.Execute("WorkflowId", "FilePath", "", "url", "clientId", "clientSecret", "resource");
            } catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No organisationId provided", ex.Message);
            }
        }
        [Fact]
        public async Task Exception_NoUrl() {
            try {
                string success = await ExecuteWorkflow.Execute("WorklowId", "filePath", "organisationId", "", "clientId", "clientSecret", "resource");
            }catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No url provided", ex.Message);
            }
        }
        [Fact]
        public async Task Exception_NoClientId() {
            try {
                string success = await ExecuteWorkflow.Execute("workflowId", "filePath", "OrganisationId", "url", "", "clientSecret", "resource");
            }catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No clientId provided", ex.Message);
            }
        }
        [Fact]
        public async Task Exception_NoClientSecret() {
            try {
                string success = await ExecuteWorkflow.Execute("WorkflowId", "filePath", "OrganisationId", "url", "ClientId", "", "resource");
            } catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No clientSecret provided", ex.Message);
            }
        }
        public async Task Exception_NoResource() {
            try {
                string success = await ExecuteWorkflow.Execute("workflowId", "filePath", "organisationId", "url", "clientId", "clientSecret", "");
            } catch(Exception ex) {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("No resource provided", ex.Message);
            }
        }
    }
}