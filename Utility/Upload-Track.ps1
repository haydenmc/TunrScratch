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
Write-Host -ForegroundColor Cyan "Fetching auth token for $Username..."
Set-ServiceProperties `
    -ServiceProtocol $ServiceProtocol `
    -ServiceHostname $ServiceHostname `
    -ServicePort $ServicePort
$token = Get-AuthToken -Username $Username -Password $Password
Write-Host -ForegroundColor Green "Authentication token: $token`n"

Write-Host -ForegroundColor Cyan "Attempting to upload file to Tunr..."
$uri = "$($ServiceProtocol)://$($ServiceHostname):$ServicePort$ServicePath"
Write-Host "File: $AudioFilePath"
Write-Host "Service Uri: $uri"
$headers = @{Authorization = "Bearer $token"}
$ProgressPreference = "SilentlyContinue" # Without this, for some reason, it takes forever.
$response = Invoke-RestMethod `
    -Headers $headers `
    -Uri $uri `
    -Method Post `
    -InFile $AudioFilePath `
    -ContentType "multipart/form-data"

return $response