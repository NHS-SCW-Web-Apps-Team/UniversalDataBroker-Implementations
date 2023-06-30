using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure.Storage.Blobs.Models;

namespace CollectFunction
{
    public static class Collect
    {
        private static string localStore = @"C:\home\site\wwwroot\localStore";
        private static HttpClient _client;

        [FunctionName("Collect")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                InputObj data = JsonConvert.DeserializeObject<InputObj>(requestBody);

                _client = new HttpClient();

                var stor = getEnvVar("StorageAccount");
                var serviceUrl = getEnvVar("ServiceUrl");
                var clientId = getEnvVar("ClientId");
                var clientSecret = getEnvVar("ClientSecret");
                var tokenResource = getEnvVar("TokenResource");


                var containerClient = new BlobContainerClient(stor, data.Container);

                var blobPath = "";

                if (!string.IsNullOrEmpty(data.Path))
                {
                    blobPath = $"{data.Path}\\{data.FileName}";
                }
                else{
                    blobPath = data.FileName;
                }
                var blob = containerClient.GetBlobClient(blobPath);
                var path = System.IO.Directory.CreateDirectory(localStore);
                var tmpFile = System.IO.Path.Combine(localStore, data.FileName);
                using (var fileStream = System.IO.File.OpenWrite(tmpFile))
                {
                    await blob.DownloadToAsync(fileStream);
                }

                var token = await AcquireToken(tokenResource, clientId, clientSecret);

                var result = await UploadFile($"{localStore}\\{data.FileName}", data.WorkflowGuid, data.OrganisationGuid, token, serviceUrl);

                deleteFile(data.FileName);

                return new OkObjectResult(result);
            }catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
        private static string getEnvVar(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
        private static void deleteFile(string fileName)
        {
            System.IO.File.Delete(System.IO.Path.Combine(localStore,fileName));
        }

        private static async Task<string> UploadFile(string filePath, string workflowId, string OrganisationId, string token, string url)
        {
            _client.DefaultRequestHeaders.Clear();
            var content = new MultipartFormDataContent();
            var fileName = Path.GetFileName(filePath);
            var reqContent = new StreamContent(new FileStream(filePath, FileMode.Open));
            var contentType = "";
            var Extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
            switch (Extension)
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
            reqContent.Dispose();
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
                var response = await _client.PostAsync(new Uri("https://login.microsoftonline.com/4088bef3-a7fb-4b63-b87b-4d8eda09b28d/oauth2/token?"), content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    string _token = json.Substring(json.IndexOf("access_token") + 15);
                    var token = _token.Substring(0, _token.IndexOf("\""));
                    return token;
                }
                else
                {
                    throw new Exception($"Unabable to get a token: {response.ReasonPhrase}");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }


    public class InputObj
    {
        public string Container { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string WorkflowGuid { get; set; }
        public string OrganisationGuid { get; set; }
    }
}
