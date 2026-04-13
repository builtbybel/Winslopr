# Create a system restore point (safety net before changes)
# Host: console
# Category: Tool
# Options: Create restore point

param([string]$choice)

if ($choice -eq "Create restore point") {
    # Check for admin rights
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()
    ).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

    if (-not $isAdmin) {
        Write-Output "You must run this Winslop script as Administrator to create a restore point."
        exit
    }

    try {
        Checkpoint-Computer -Description "Winslop Restore Point" -RestorePointType "MODIFY_SETTINGS"
        Write-Output "✔ Restore point created successfully."
    }
    catch {
        Write-Output "✘ Failed to create restore point: $($_.Exception.Message)"
    }
}
