Set-Location $PSScriptRoot

$port = 5107
$url = "http://localhost:$port"

$listener = netstat -ano | Select-String ":$port\s"
if ($listener) {
    Write-Host "OpsPilot is already running at $url"
    Start-Process $url
    exit 0
}

Write-Host "Starting OpsPilot Web at $url ..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$PSScriptRoot'; dotnet run --project .\OpsPilot.Web\OpsPilot.Web.csproj --urls '$url'"

$maxAttempts = 30
for ($i = 1; $i -le $maxAttempts; $i++) {
    Start-Sleep -Seconds 1
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -MaximumRedirection 0 -ErrorAction Stop
        if ($response.StatusCode -ge 200) {
            break
        }
    }
    catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode.value__ -eq 302) {
            break
        }
    }
}

$finalCheck = netstat -ano | Select-String ":$port\s"
if (-not $finalCheck) {
    Write-Error "OpsPilot failed to start on $url"
    exit 1
}

Write-Host "OpsPilot started successfully: $url"
Start-Process $url
