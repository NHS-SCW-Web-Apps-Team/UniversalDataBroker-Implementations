namespace Implementations.Dotnet.UploadFile
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class ExecuteWorkflow
    {
        private static HttpClient _client;

        public static async Task<string> Execute(string workflowid, string filePath, string OrganisationId, string url, string clientId, string clientSecret, string resource)
        {
            if(string.IsNullOrWhiteSpace(workflowid)) {
                throw new InvalidOperationException("No workflowId provided");
            }
            if(string.IsNullOrWhiteSpace(filePath)) {
                throw new InvalidOperationException("No filepath provided");
            }
            if (string.IsNullOrWhiteSpace(OrganisationId)) {
                throw new InvalidOperationException("No organisationId provided");
            }
            if(string.IsNullOrWhiteSpace(url)) {
                throw new InvalidOperationException("No url provided");
            }
            if (string.IsNullOrWhiteSpace(clientId)) {
                throw new InvalidOperationException("No clientId provided");
            }
            if (string.IsNullOrWhiteSpace(clientSecret)) {
                throw new InvalidOperationException("No clientSecret provided");
            }
            if(string.IsNullOrWhiteSpace(resource)) {
                throw new InvalidOperationException("No resource provided");
            }
            _client = new HttpClient();
            var token = await AcquireToken(resource, clientId, clientSecret);
            return await UploadFile(filePath, workflowid, OrganisationId, token, url);
        }

        private static async Task<string> UploadFile(string filePath, string workflowId, string OrganisationId, string token, string url)
        {
            _client.DefaultRequestHeaders.Clear();
            var content = new MultipartFormDataContent();
            var fileName = Path.GetFileName(filePath);
            var reqContent = new StreamContent(new FileStream(filePath, FileMode.Open));
            var contentType = "";
            var Extension = fileName.Substring(fileName.LastIndexOf('.') +1);
            switch(Extension)
            {
                case "csv":
                    contentType = "text/csv";
                    break;
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "json":
                    contentType = "application/json";
                    break;
                case "zip":
                    contentType = "application/zip";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            reqContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(reqContent, fileName, fileName);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync($"{url}/api/files/upload/{OrganisationId}/{workflowId}", content);
            var respContent = await response.Content.ReadAsStringAsync();
            return respContent;
        }

        private static async Task<string> AcquireToken(string url, string clientId, string clientSecret)
        {
            try
            {
                var content = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>(
                            "grant_type", "client_credentials"
                        ),
                        new KeyValuePair<string, string>(
                            "client_id", clientId
                        ),
                        new KeyValuePair<string, string>(
                            "client_secret", clientSecret
                        ),
                        new KeyValuePair<string, string>(
                            "resource", url
                        )
                    }
                );

                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                _client.DefaultRequestHeaders.Clear();
                var response = await _client.PostAsync(new Uri("https://login.microsoftonline.com//oauth2/token?"), content);
                if (response.IsSuccessStatusCode){
                    var json = await response.Content.ReadAsStringAsync();
                    string _token = json.Substring(json.IndexOf("access_token")+15);
                    var token = _token.Substring(0, _token.IndexOf("\""));
                    return token;
                } else {
                    throw new Exception($"Unabable to get a token: {response.ReasonPhrase}");
                }
            }
            catch(Exception e) {
                throw e;
            }
        }
    }
}
