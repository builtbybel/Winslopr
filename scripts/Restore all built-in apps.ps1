# Restore all built-in apps - Bring back all default Windows apps with a single command.
# Category: Tool

# Check if running as Administrator
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Warning "This script must be run as Administrator."
    Start-Process powershell.exe -Verb RunAs -ArgumentList ("-File `"{0}`"" -f $MyInvocation.MyCommand.Path)
    exit
}

# Get all installed Windows apps for all users
$allApps = Get-AppxPackage -AllUsers

# Counter for tracking progress
$totalApps = $allApps.Count
$currentAppIndex = 1
$reinstalledCount = 0

# Loop through each app and reinstall
foreach ($app in $allApps) {
    # Display current status
    Write-Host "[$currentAppIndex / $totalApps] Reinstalling $($app.Name)..." -ForegroundColor Yellow

    # Try to reinstall the app
    try {
        Add-AppxPackage -Register "$($app.InstallLocation)\appxmanifest.xml" -DisableDevelopmentMode -ErrorAction Stop
        Write-Host "[$currentAppIndex / $totalApps] $($app.Name) reinstalled successfully." -ForegroundColor Green
        $reinstalledCount++
    }
    catch {
        Write-Host "[$currentAppIndex / $totalApps] Error occurred while reinstalling $($app.Name): $_" -ForegroundColor Red
    }

    # Increment the counter
    $currentAppIndex++
}

# Display final status
if ($reinstalledCount -eq $totalApps) {
    Write-Host "All apps reinstalled successfully." -ForegroundColor Green
    Write-Host "Flyby11 has reinstalled everything possible." -ForegroundColor Cyan
    # Add success percentage reporting
    $successPercentage = [math]::Round(($reinstalledCount / $totalApps) * 100, 2)
    Write-Host "Completed with $successPercentage% success rate ($reinstalledCount of $totalApps apps reinstalled)" -ForegroundColor Cyan
}