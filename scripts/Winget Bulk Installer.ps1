# Description: Install multiple apps via winget in one go.
# Category: Post
# Options: Install packages
# Host: log
# Input: true
# InputPlaceholder: Enter package IDs separated by space/comma/newline (e.g., Microsoft.PowerToys Git.Git)

[CmdletBinding()]
param(
  [string]$Option,
  [string]$ArgsText
)

if ($Option -ne 'Install packages') { throw "Unknown option. Use 'Install packages'." }

# --- Resolve winget.exe (PS5-compatible) ---
$winget = $null
try {
  $cmd = Get-Command winget.exe -ErrorAction Stop
  if ($cmd -and $cmd.Path) { $winget = $cmd.Path }
} catch { }

if (-not $winget) {
  $tryLocalApps = Join-Path $env:LOCALAPPDATA 'Microsoft\WindowsApps\winget.exe'
  if (Test-Path $tryLocalApps) { $winget = $tryLocalApps }
}

if (-not $winget) {
  $tryProgApps = Join-Path $env:ProgramFiles 'WindowsApps\Microsoft.DesktopAppInstaller_8wekyb3d8bbwe\winget.exe'
  if (Test-Path $tryProgApps) { $winget = $tryProgApps }
}

if (-not $winget) { throw "winget.exe not found. Please install App Installer from Microsoft Store." }

# --- Split package list by newline/comma/semicolon/space ---
$ids = $ArgsText -split "[\r\n,; ]+" | Where-Object { $_ -and $_.Trim().Length -gt 0 } | ForEach-Object { $_.Trim() }
if ($ids.Count -eq 0) { throw "No package IDs provided." }

foreach ($id in $ids) {
  Write-Output "Installing: $id"
  & $winget install $id -e --accept-source-agreements --accept-package-agreements
}
