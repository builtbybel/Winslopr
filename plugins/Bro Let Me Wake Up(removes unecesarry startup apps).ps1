$configPath = ".\BLMWP_CONFIG.json"

if (!(Test-Path $configPath)) {
    Write-Host "Config file not found!" -ForegroundColor Red
    exit
}

$config = Get-Content $configPath | ConvertFrom-Json

$startupApprovedPaths = @(
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run",
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder",
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run32"
)

# Byte values
$disableValue = [byte[]](3,0,0,0,0,0,0,0) #cuz it disables apps from starting up so it lets yo damn pc WAKE UP.

foreach ($app in $config.apps) {
    Write-Host "`nProcessing $app ..." -ForegroundColor Cyan

    $found = $false

    foreach ($path in $startupApprovedPaths) {
        if (!(Test-Path $path)) { continue }

        $items = Get-ItemProperty -Path $path

        foreach ($prop in $items.PSObject.Properties) {
            if ($prop.Name -match $app) {
                $found = $true
                Set-ItemProperty -Path $path -Name $prop.Name -Value $disableValue
                Write-Host "Disabled: $($prop.Name)" -ForegroundColor Yellow
            }
        }
    }

    if (-not $found) {
        Write-Host "Not found: $app" -ForegroundColor Red
# bruther let me WAKE UP
        if ($config.abortIfMissing) {
            Write-Host "Aborting..." -ForegroundColor Red
            exit
    }
}