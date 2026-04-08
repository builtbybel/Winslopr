# Clear Icon Cache – This script clears the Windows icon cache to fix incorrectly displayed or missing icons.
# Category: Tool
# Host: log

Write-Output "Refreshing system icons..."
Start-Process "ie4uinit.exe" -ArgumentList "-show" -Wait

Write-Output "Terminating Windows Explorer..."
Stop-Process -Name "explorer" -Force

# Delete the main IconCache.db file
$iconCachePath = "$env:LOCALAPPDATA\IconCache.db"
if (Test-Path $iconCachePath) {
    Write-Output "Removing IconCache.db..."
    Remove-Item $iconCachePath -Force -ErrorAction SilentlyContinue
} else {
    Write-Output "IconCache.db not found – skipping."
}

# Delete all iconcache* files in the Explorer folder
$explorerCachePath = "$env:LOCALAPPDATA\Microsoft\Windows\Explorer\iconcache*"
Write-Output "Cleaning Explorer iconcache files..."
Get-ChildItem -Path $explorerCachePath -Force -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue

Write-Output "Restarting Windows Explorer..."
Start-Process "explorer.exe"

Write-Output "Icon cache cleared successfully. Icons should now refresh."
