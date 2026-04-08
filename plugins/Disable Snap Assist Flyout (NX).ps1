# ================================================================================
# 📁 Disable Snap Assist Flyout Plugin
# This plugin disables the Snap Assist Flyout feature in Windows 11.
#
# ✅ Useful for users who prefer not to see Snap Assist Flyout.
# 🔄 Undo restores Snap Assist Flyout to enabled.
# ================================================================================

[Commands]
Info=This plugin disables Snap Assist Flyout in Windows 11
Check=reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v EnableSnapAssistFlyout
Do=reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v EnableSnapAssistFlyout /t REG_DWORD /d 0 /f && taskkill /f /im explorer.exe && start explorer.exe
Undo=reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v EnableSnapAssistFlyout /t REG_DWORD /d 1 /f && taskkill /f /im explorer.exe && start explorer.exe

[Expect]
EnableSnapAssistFlyout=0x0

# Script Body (optional)
Write-Host "Plugin 'DisableSnapAssistFlyout' executed."
