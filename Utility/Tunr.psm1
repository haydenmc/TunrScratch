<#
    Tunr PowerShell module
#>

$script:ServiceProtocol = "http"
$script:ServiceHostname = "localhost"
$script:ServicePort     = "5000"

function Set-ServiceProperties
{
    param(
        $ServiceProtocol = $script:ServiceProtocol,
        $ServiceHostname = $script:ServiceHostname,
        $ServicePort = $script:ServicePort
    )
    $script:ServiceProtocol = $ServiceProtocol
    $script:ServiceHostname = $ServiceHostname
    $script:ServicePort = $ServicePort
}

function Get-AuthWebSession
{
    param(
        # Username
        [Parameter(Mandatory = $true, Position = 0)]
        [string]
        [ValidateNotNullOrEmpty()]
        $Username,
        # Password
        [Parameter(Mandatory = $true, Position = 1)]
        [string]
        [ValidateNotNullOrEmpty()]
        $Password,
        # Token endpoint
        [Parameter()]
        [string]
        [ValidateNotNullOrEmpty()]
        $TokenEndpoint = "/User/Login"
    )

    $uri = "$($script:ServiceProtocol)://$($script:ServiceHostname):$($script:ServicePort)$TokenEndpoint"
    $body = @{
        "email" = $Username
        "password" = $Password
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Method Post -Uri $uri -ContentType "application/json" -Body $body -SessionVariable "sessionResult"
    return $sessionResult
}

Export-ModuleMember `
    Set-ServiceProperties,`
    Get-AuthWebSession
