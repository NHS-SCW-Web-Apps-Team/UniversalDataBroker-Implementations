param(
    [Paramenter()]
    [string]$tenant,
    [Parameter()]
    [string]$ClientId,
    [Parameter()]
    [string]$ClientSecret,
    [Parameter()]
    [string]$OrganisationId,
    [Parameter()]
    [string]$Resource,
    [Parameter()]
    [string]$Url,
    [Parameter()]
    [string]$WorkflowId,
    [Parameter()]
    [string]$FilePath
)

$tokenBody = @{grant_type='client_credentials'
                client_id=$ClientId
                client_secret=$ClientSecret
                resource=$resource};
$tokenContentType = 'application/x-www-form-urlencoded';

$tokenResponse = Invoke-RestMethod -Uri "https://login.microsoftonline.com/{$tenant}/oauth2/token?" -ContentType $tokenContentType -Method Post -Body $tokenBody

$token = $tokenResponse.access_token;

Write-Host $token;

$fileName = (Get-ChildItem $filePath).BaseName;
$fileType = (Get-ChildItem $filePath).Extension;

$contentType = "application/octet-stream";
if($fileType -eq ".csv"){
    $contentType = "text/csv";
}
if($fileType -eq ".xls"){
    $contentType = "application/vnd.ms-excel";
}
if($fileType -eq ".json"){
    $contentType = "application/json";
}
if($fileType -eq ".zip"){
    $contentType = "application/zip";
}

Write-Host $contentType;

$multipartContent = [System.Net.Http.MultipartFormDataContent]::new()
$FileStream = [System.IO.FileStream]::new($FilePath, [System.IO.FileMode]::Open)
$fileHeader = [System.Net.Http.Headers.ContentDispositionHeaderValue]::new("form-data")
$fileHeader.Name = $fileName
$fileHeader.FileName = $fileName + $fileType
$fileContent = [System.Net.Http.StreamContent]::new($FileStream)
$fileContent.Headers.ContentDisposition = $fileHeader
$fileContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse($contentType)
$multipartContent.Add($fileContent)

$headers = @{
    Authorization = "Bearer ${token}"
};

$postUri = "${Url}/api/files/upload/${OrganisationId}/${WorkflowId}";

$response = Invoke-RestMethod -Uri $postUri -Headers $headers -Method Post -Body $multipartContent

Write-Host $response