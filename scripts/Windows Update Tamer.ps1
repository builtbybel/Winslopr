# Windows 11 Updates Manager
# Host: log
# Category: Tool
# Options: Show Info; Pause Updates; Resume Updates; Disable Automatic Updates; Set Max Pause Time; Show Status; Revert Defaults
# Credits: https://woshub.com/pause-delay-windows-updates/

param([string]$choice)

function Ensure-UxKey {
    $ux = "HKLM:\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"
    if (-not (Test-Path $ux)) { New-Item -Path $ux -Force | Out-Null }
    return $ux
}

function Unlock-PauseUx {
    $pol = "HKLM:\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate"
    if (Test-Path $pol) {
        Remove-ItemProperty -Path $pol -Name "SetDisablePauseUXAccess" -ErrorAction SilentlyContinue
    }
}

switch ($choice) {

    "Show Info" {
        Write-Output "[INFO] Windows Update Tamer"
        Write-Output "---------------------------------------------------"
        Write-Output "Control Windows Update without opening Settings."
        Write-Output ""
        Write-Output "Options:"
        Write-Output "- Pause Updates : Pauses updates for 35 days (sets Feature/Quality start/end and expiry)."
        Write-Output "- Resume Updates : Clears pause values."
        Write-Output "- Disable Automatic Updates : Policy-based turn off of auto updates."
        Write-Output "- Set Max Pause Time : Sets a far-future expiry (10 years) and matching Feature/Quality end times."
        Write-Output "- Show Status : Prints current pause dates."
        Write-Output "- Revert Defaults : Removes policy and pause values."
        Write-Output ""
        Write-Output "Notes:"
        Write-Output "- Admin rights required."
        Write-Output "- Works on Windows 10/11."
        Write-Output "- Based on registry values described by Windows OS Hub."
    }

    "Pause Updates" {
        Write-Output "[ACTION] Pausing updates for 35 days..."
        Unlock-PauseUx
        $ux = Ensure-UxKey
        $start = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        $end   = (Get-Date).AddDays(35).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

        Set-ItemProperty -Path $ux -Name "PauseUpdatesStartTime"           -Value $start
        Set-ItemProperty -Path $ux -Name "PauseUpdatesExpiryTime"          -Value $end
        Set-ItemProperty -Path $ux -Name "PauseFeatureUpdatesStartTime"    -Value $start
        Set-ItemProperty -Path $ux -Name "PauseFeatureUpdatesEndTime"      -Value $end
        Set-ItemProperty -Path $ux -Name "PauseQualityUpdatesStartTime"    -Value $start
        Set-ItemProperty -Path $ux -Name "PauseQualityUpdatesEndTime"      -Value $end

        Write-Output "[OK] Updates paused until $((Get-Date).AddDays(35).ToShortDateString())."
        Write-Output "[TIP] Close and reopen the Settings app to refresh the date."
    }

    "Resume Updates" {
        Write-Output "[ACTION] Resuming Windows Updates..."
        $ux = Ensure-UxKey
        $names = @(
            "PauseUpdatesStartTime","PauseUpdatesExpiryTime",
            "PauseFeatureUpdatesStartTime","PauseFeatureUpdatesEndTime",
            "PauseQualityUpdatesStartTime","PauseQualityUpdatesEndTime"
        )
        foreach ($n in $names) {
            Remove-ItemProperty -Path $ux -Name $n -ErrorAction SilentlyContinue
        }
        Write-Output "[OK] Pause cleared."
    }

    "Disable Automatic Updates" {
        Write-Output "[ACTION] Disabling automatic updates via policy..."
        $au = "HKLM:\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU"
        New-Item -Path $au -Force | Out-Null
        Set-ItemProperty -Path $au -Name "NoAutoUpdate" -Type DWord -Value 1
        Write-Output "[OK] Automatic updates disabled (manual checks still possible)."
    }

    "Set Max Pause Time" {
        Write-Output "[ACTION] Extending pause to 10 years..."
        Unlock-PauseUx
        $ux = Ensure-UxKey
        $start = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        $end   = (Get-Date).AddDays(3650).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

        # set all pause timestamps
        Set-ItemProperty -Path $ux -Name "PauseUpdatesStartTime"           -Value $start
        Set-ItemProperty -Path $ux -Name "PauseUpdatesExpiryTime"          -Value $end
        Set-ItemProperty -Path $ux -Name "PauseFeatureUpdatesStartTime"    -Value $start
        Set-ItemProperty -Path $ux -Name "PauseFeatureUpdatesEndTime"      -Value $end
        Set-ItemProperty -Path $ux -Name "PauseQualityUpdatesStartTime"    -Value $start
        Set-ItemProperty -Path $ux -Name "PauseQualityUpdatesEndTime"      -Value $end

        # optional: expand UI's selectable max pause days (not required for actual pause)
        New-Item -Path $ux -Force | Out-Null
        New-ItemProperty -Path $ux -Name "FlightSettingsMaxPauseDays" -PropertyType DWord -Value 36500 -Force | Out-Null

        Write-Output "[OK] Pause extended to $((Get-Date).AddDays(3650).ToShortDateString())."
        Write-Output "[TIP] Settings app may need to be restarted to reflect the new date."
    }

    "Show Status" {
        $ux = "HKLM:\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"
        if (Test-Path $ux) {
            Get-ItemProperty -Path $ux |
            Select-Object PauseUpdatesStartTime,PauseUpdatesExpiryTime,PauseFeatureUpdatesStartTime,PauseFeatureUpdatesEndTime,PauseQualityUpdatesStartTime,PauseQualityUpdatesEndTime,FlightSettingsMaxPauseDays |
            Format-List
        } else {
            Write-Output "[INFO] UX Settings key not found."
        }
    }

    "Revert Defaults" {
        Write-Output "[ACTION] Reverting to defaults..."
        Remove-Item -Path "HKLM:\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate" -Recurse -ErrorAction SilentlyContinue
        $ux = "HKLM:\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"
        if (Test-Path $ux) {
            $names = @(
                "FlightSettingsMaxPauseDays",
                "PauseUpdatesStartTime","PauseUpdatesExpiryTime",
                "PauseFeatureUpdatesStartTime","PauseFeatureUpdatesEndTime",
                "PauseQualityUpdatesStartTime","PauseQualityUpdatesEndTime"
            )
            foreach ($n in $names) {
                Remove-ItemProperty -Path $ux -Name $n -ErrorAction SilentlyContinue
            }
        }
        Write-Output "[OK] Default behavior restored."
    }

    default {
        Write-Output "[ERROR] Unknown option: $choice"
    }
}
