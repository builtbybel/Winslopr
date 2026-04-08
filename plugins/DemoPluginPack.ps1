# ================================================================================
# 🧪 Demo Plugin Pack
# Demonstrates all currently supported features of the plugin system
# ================================================================================
# ✅ Features demonstrated:
# - Metadata section: [Plugin] with Info
# - Multiple commands: [Commands] with Check / Do / Undo definitions
# - Value validation: [Expect] to define expected output values from commands
# - Multi-line registry checks, including multiple keys
# - Optional command output logging (e.g., `whoami`)
# - Optional script logic (executed like a normal PowerShell script)
#
# 💡 Usage notes:
# - The Check command is executed first; its output is parsed against [Expect]
# - The Do command is run when fixing is triggered
# - The Undo command attempts to revert changes
# - Output from all commands is logged, and key-value validation is shown
# - Keys not listed under [Expect] are ignored in validation
#
# 📁 File name: DemoPluginPack.ps1
# =================================================================================

[Commands]
Info=Demonstrates all available plugin features including Registry, System, and File operations
Check=reg query HKLM\Software\Policies\Microsoft\Windows\DataCollection /v AllowTelemetry && reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Search /v BingSearchEnabled && whoami
Do=reg add HKLM\Software\Policies\Microsoft\Windows\DataCollection /v AllowTelemetry /t REG_DWORD /d 0 /f && reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Search /v BingSearchEnabled /t REG_DWORD /d 0 /f && echo Fixes applied
Undo=reg add HKLM\Software\Policies\Microsoft\Windows\DataCollection /v AllowTelemetry /t REG_DWORD /d 1 /f && reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Search /v BingSearchEnabled /t REG_DWORD /d 1 /f && echo Reverted

[Expect]
AllowTelemetry=0x0
BingSearchEnabled=0x0
# whoami output is not validated – only logged

# Optional script body – this executes if the script is run directly
Write-Host "This is the demo plugin pack. Logic can be placed here if needed."

