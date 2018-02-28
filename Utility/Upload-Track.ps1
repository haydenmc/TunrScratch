<#
    Script to upload a track to Tunr using web API
#>

param(
    # Audio file to upload
    [Parameter(Position = 0, Mandatory = $true)]
    [string]
    [ValidateNotNullOrEmpty()]
    $AudioFilePath,
    # Email of user to authenticate
    [Parameter(Mandatory = $true)]
    [string]
    [ValidateNotNullOrEmpty()]
    $Username,
    # Password of user to authenticate
    [Parameter(Mandatory = $true)]
    [string]
    [ValidateNotNullOrEmpty()]
    $Password,
    # Protocol of service
    [Parameter()]
    [ValidateSet("http", "https")]
    [string]
    $ServiceProtocol = "http",
    # Hostname of service
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $ServiceHostname = "localhost",
    # Port number of service
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $ServicePort = "5000",
    # Path of service endpoint
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $ServicePath = "/Library"
)

Import-Module "$PSScriptRoot\Tunr"

# Get auth token
Write-Host -ForegroundColor Cyan "Fetching auth cookie for $Username..."
Set-ServiceProperties `
    -ServiceProtocol $ServiceProtocol `
    -ServiceHostname $ServiceHostname `
    -ServicePort $ServicePort
$authSession = Get-AuthWebSession -Username $Username -Password $Password
Write-Host -ForegroundColor Green "Cookies returned: $($authSession.Cookies.Count)`n"

Write-Host -ForegroundColor Cyan "Attempting to upload file to Tunr..."
$uri = "$($ServiceProtocol)://$($ServiceHostname):$ServicePort$ServicePath"

$fileName = Split-Path $AudioFilePath -leaf
$boundary = "abcdefghijklmnop"
$fileBin = [System.IO.File]::ReadAllBytes($AudioFilePath)
$enc = [System.Text.Encoding]::GetEncoding("iso-8859-1")
$contentType = "application/octet-stream"
$bodyTemplate = @'
--{0}
Content-Disposition: form-data; name="files"; filename="{1}"
Content-Type: {2}

{3}
--{0}--

'@

$body = $bodyTemplate -f $boundary, $fileName, $contentType, $enc.GetString($fileBin)

Write-Host "File: $fileName"
Write-Host "Service Uri: $uri"

$ProgressPreference = "SilentlyContinue" # Without this, for some reason, it takes forever.
$response = Invoke-WebRequest `
    -WebSession $authSession `
    -Uri $uri `
    -Method Post `
    -Body $body `
    -ContentType "multipart/form-data; boundary=$boundary" `
    -UseBasicParsing

return $response.Content
