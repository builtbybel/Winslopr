# Power Actions – Restart Explorer, Restart, Shutdown, Logoff, Sleep, Hibernate
# Host: silent
# Category: Post
# Options: Restart Explorer; Restart PC; Shutdown PC; Log off; Sleep; Hibernate

param([string]$choice)

switch ($choice) {

    "Restart Explorer" {
        Write-Output "Restarting Explorer..."
        Stop-Process -Name explorer -Force
        Start-Process explorer.exe
    }

    "Restart PC" {
        Write-Output "Restarting computer..."
        Restart-Computer -Force
    }

    "Shutdown PC" {
        Write-Output "Shutting down computer..."
        Stop-Computer -Force
    }

    "Log off" {
        Write-Output "Logging off current user..."
        shutdown.exe /l
    }

    "Sleep" {
        Write-Output "Putting system to sleep..."
        rundll32.exe powrprof.dll,SetSuspendState 0,1,0
    }

    "Hibernate" {
        Write-Output "Hibernating system..."
        rundll32.exe powrprof.dll,SetSuspendState Hibernate
    }

    default {
        Write-Output "Unknown option: $choice"
    }
}
