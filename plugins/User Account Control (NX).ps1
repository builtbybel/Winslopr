# ================================================================================
# 🔐 Enable User Account Control (UAC)
# This plugin enables or disables UAC.
# Disabling UAC is risky and should only be done if you fully control the system.
# 🔄 Undo will disable UAC again.
# ⚠️ Requires system restart to apply changes.
# ================================================================================

[Commands]
Info=Enable or disable User Account Control (UAC). Disabling this can reduce system security! 
Check=reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA
Do=powershell -Command "Add-Type -AssemblyName PresentationCore,PresentationFramework; $result = [System.Windows.MessageBox]::Show('Your system will restart in 30 seconds to apply changes. Do you want to proceed?', 'Warning', 'YesNo', 'Warning'); if ($result -eq 'Yes') { Start-Sleep -Seconds 30; reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA /t REG_DWORD /d 1 /f; shutdown /r /t 0 }"
Undo=powershell -Command "Add-Type -AssemblyName PresentationCore,PresentationFramework; $result = [System.Windows.MessageBox]::Show('Your system will restart in 30 seconds to apply changes. Do you want to proceed?', 'Warning', 'YesNo', 'Warning'); if ($result -eq 'Yes') { Start-Sleep -Seconds 30; reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA /t REG_DWORD /d 0 /f; shutdown /r /t 0 }"

[Expect]
EnableLUA=0x1

# Script Body (optional)
Write-Host "Plugin 'EnableUAC' executed."
