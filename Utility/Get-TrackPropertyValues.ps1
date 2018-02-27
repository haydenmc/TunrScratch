<#
    Script to upload a track to Tunr using web API
#>

param(
    # Audio file to upload
    [Parameter(Position = 0, Mandatory = $true)]
    [string]
    [ValidateNotNullOrEmpty()]
    $PropertyName,
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
    $ServicePath = "/Library/Track"
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

Write-Host -ForegroundColor Cyan "Attempting to fetch track property values for '$PropertyName'..."
$uri = "$($ServiceProtocol)://$($ServiceHostname):$ServicePort$ServicePath/$PropertyName"

$response = Invoke-WebRequest `
    -WebSession $authSession `
    -Uri $uri `
    -Method Get `
    -UseBasicParsing

return $response.Content