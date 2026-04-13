# RemoveWindowsAI – Force remove Windows AI features
# Host: log
# Category: Tool
# Description: Removes or disables Windows 11 AI components such as Copilot, Recall, AI Actions, and related services. Must be run as Administrator. Windows PowerShell 5.1 recommended.
# Options: Run utility
# PoweredBy: Powered by zoicware (RemoveWindowsAI)
# PoweredUrl: https://github.com/zoicware/RemoveWindowsAI

param([string]$choice)

$scriptUrl = "https://raw.githubusercontent.com/zoicware/RemoveWindowsAI/main/RemoveWindowsAi.ps1"

switch ($choice) {
    "Run utility" {
        Write-Output "Downloading and launching RemoveWindowsAI..."

        try {
            # Check for Administrator privileges
            $isAdmin = ([Security.Principal.WindowsPrincipal] `
                [Security.Principal.WindowsIdentity]::GetCurrent() `
            ).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

            if (-not $isAdmin) {
                Write-Output "WARNING: Script is not running as Administrator. Some actions may fail."
            }

            # Execute the remote script (recommended method by the project)
            & ([scriptblock]::Create((irm $scriptUrl)))

            Write-Output "SUCCESS: RemoveWindowsAI launched."
        }
        catch {
            Write-Output "ERROR: Failed to launch RemoveWindowsAI."
            Write-Output $_.Exception.Message
        }
    }

    default {
        Write-Output "Unknown option: $choice"
    }
}
