param(
    [string]$tenant,
    [string]$ClientId,
    [string]$ClientSecret,
    [string]$OrganisationId,
    [string]$Resource,
    [string]$Url,
    [string]$WorkflowId,
    [string]$data
    )

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12



$tokenBody = @{grant_type='client_credentials'
                client_id=$ClientId
                client_secret=$ClientSecret
                resource=$resource};
$tokenContentType = 'application/x-www-form-urlencoded';

$tokenResponse = Invoke-RestMethod -Uri "https://login.microsoftonline.com/{$tenant}/oauth2/token?" -ContentType $tokenContentType -Method Post -Body $tokenBody

$token = $tokenResponse.access_token;

#Write-Host $token;

$contentType = "application/json";

$headers = @{
    Authorization = "Bearer ${token}"
    ContentType = $contentType
    Accept = "text/plain"
};

$postUri = "${Url}/api/files/collect/${OrganisationId}/${WorkflowId}";

$body = "{'description':'$data','data':'$data'}"
#Write-Host $body

#Write-Host $postUri

$response = Invoke-RestMethod -Uri $postUri -Headers $headers -Method Post -Body $body

Write-Host $response