# Chris Titus Tech's Windows Utility – Install Programs, Tweaks, Fixes, Updates
# Host: log
# Category: Tool
# Options: Run utility
# PoweredBy: Powered by Chris Titus Tech
# PoweredUrl: https://christitus.com

param([string]$choice)

switch ($choice) {
    "Run utility" {
        Write-Output "Downloading and launching Chris Titus Tech's Windows Utility..."
        try {
            # Invoke the remote script
            irm christitus.com/win | iex
            Write-Output "✔ Chris Titus Utility launched successfully."
        } catch {
            Write-Output "✘ Failed to launch utility: $($_.Exception.Message)"
        }
    }
    default {
        Write-Output "Unknown option: $choice"
    }
}
