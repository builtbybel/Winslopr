# ================================================================================
# 📁 Hide File Extensions Plugin
# This plugin enables or disables the hiding of known file extensions in Explorer.
# It also controls visibility of super hidden files.
#
# ✅ Useful for users who want a cleaner file view.
# 🔄 Undo restores original visibility settings.
# ================================================================================

[Commands]
Info=This plugin hides known file extensions and disables viewing super hidden files
Check=reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v HideFileExt && reg query HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v ShowSuperHidden
Do=reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v HideFileExt /t REG_DWORD /d 1 /f 
&& reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v ShowSuperHidden /t REG_DWORD /d 0 /f && taskkill /f /im explorer.exe && start explorer.exe
Undo=reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v HideFileExt /t REG_DWORD /d 0 /f 
&& reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced /v ShowSuperHidden /t REG_DWORD /d 1 /f && taskkill /f /im explorer.exe && start explorer.exe

[Expect]
HideFileExt=0x1
ShowSuperHidden=0x0

# Script Body (optional)
Write-Host "Plugin 'HideFileExtensions' executed."
