# Description: Download and run Windows 11 Media Creation Tool (MCT) and Rufus (USB creator).
# Category: Tool
# Host: embedded
# Options: Download MCT; Download + Run MCT; Run downloaded MCT; Open Microsoft Windows 11 download page; Delete downloaded MCT; Download Rufus (portable x64); Download + Run Rufus (portable x64); Run downloaded Rufus (portable x64); Download Rufus (standard x64); Download + Run Rufus (standard x64); Run downloaded Rufus (standard x64); Open Rufus Releases; Delete downloaded Rufus

param([string]$Option, [string]$ArgsText)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

# --- Windows 11 MCT ---
$MctPage   = "https://www.microsoft.com/software-download/windows11"
$MctDirect = "https://go.microsoft.com/fwlink/?linkid=2156295"

# --- Rufus ---
$RufusPortableUrl = "https://github.com/pbatard/rufus/releases/download/v4.12/rufus-4.12p.exe"
$RufusStandardUrl = "https://github.com/pbatard/rufus/releases/download/v4.12/rufus-4.12.exe"
$RufusReleasesPage = "https://github.com/pbatard/rufus/releases"

function Ensure-Tls12 {
  try { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 } catch { }
}

function Get-StoreDir {
  $dir = Join-Path $env:TEMP "Winslop"
  if (-not (Test-Path $dir)) { New-Item -Path $dir -ItemType Directory | Out-Null }
  $dir
}

# ---------------- MCT helpers ----------------
function Get-MctPath {
  Join-Path (Get-StoreDir) "MediaCreationTool_Win11.exe"
}

function Download-Mct {
  Ensure-Tls12
  $out = Get-MctPath

  Write-Host "Downloading Windows 11 MCT..."
  Write-Host "Source: $MctDirect"
  Write-Host "Target: $out"

  Invoke-WebRequest -Uri $MctDirect -OutFile $out -UseBasicParsing

  if (-not (Test-Path $out)) { throw "Download failed: file not found after download." }

  $size = (Get-Item $out).Length
  Write-Host ("Download complete ({0} bytes)." -f $size)
  return $out
}

function Run-Mct {
  param([string]$Path)

  if ([string]::IsNullOrWhiteSpace($Path)) { throw "Run-Mct: Path is empty." }
  if (-not (Test-Path $Path)) { throw "MCT not found: $Path (use 'Download MCT' first)" }

  Write-Host "Starting MCT: $Path"
  Start-Process -FilePath $Path
}

# ---------------- Rufus helpers ----------------
function Get-RufusPortablePath {
  Join-Path (Get-StoreDir) "rufus-portable-x64.exe"
}

function Get-RufusStandardPath {
  Join-Path (Get-StoreDir) "rufus-standard-x64.exe"
}

function Download-RufusPortable {
  Ensure-Tls12
  $out = Get-RufusPortablePath

  Write-Host "Downloading Rufus (portable x64)..."
  Write-Host "Source: $RufusPortableUrl"
  Write-Host "Target: $out"

  Invoke-WebRequest -Uri $RufusPortableUrl -OutFile $out -UseBasicParsing

  if (-not (Test-Path $out)) { throw "Download failed: file not found after download." }

  $size = (Get-Item $out).Length
  Write-Host ("Download complete ({0} bytes)." -f $size)
  return $out
}

function Download-RufusStandard {
  Ensure-Tls12
  $out = Get-RufusStandardPath

  Write-Host "Downloading Rufus (standard x64)..."
  Write-Host "Source: $RufusStandardUrl"
  Write-Host "Target: $out"

  Invoke-WebRequest -Uri $RufusStandardUrl -OutFile $out -UseBasicParsing

  if (-not (Test-Path $out)) { throw "Download failed: file not found after download." }

  $size = (Get-Item $out).Length
  Write-Host ("Download complete ({0} bytes)." -f $size)
  return $out
}

function Run-Rufus {
  param([string]$Path)

  if ([string]::IsNullOrWhiteSpace($Path)) { throw "Run-Rufus: Path is empty." }
  if (-not (Test-Path $Path)) { throw "Rufus not found: $Path (download it first)" }

  Write-Host "Starting Rufus: $Path"
  Start-Process -FilePath $Path
}

# ---------------- main ----------------
if ([string]::IsNullOrWhiteSpace($Option)) { $Option = "Download + Run MCT" }

switch ($Option) {

  # ---- MCT ----
  "Download MCT" {
    $p = Download-Mct
    Write-Host "Saved to: $p"
    break
  }

  "Download + Run MCT" {
    $p = Download-Mct
    Run-Mct -Path $p
    break
  }

  "Run downloaded MCT" {
    $p = Get-MctPath
    Run-Mct -Path $p
    break
  }

  "Open Microsoft Windows 11 download page" {
    Write-Host "Opening: $MctPage"
    Start-Process $MctPage
    break
  }

  "Delete downloaded MCT" {
    $p = Get-MctPath
    if (Test-Path $p) {
      Remove-Item -LiteralPath $p -Force
      Write-Host "Deleted: $p"
    } else {
      Write-Host "Nothing to delete. File not found: $p"
    }
    break
  }

  # ---- Rufus (portable) ----
  "Download Rufus (portable x64)" {
    $p = Download-RufusPortable
    Write-Host "Saved to: $p"
    break
  }

  "Download + Run Rufus (portable x64)" {
    $p = Download-RufusPortable
    Run-Rufus -Path $p
    break
  }

  "Run downloaded Rufus (portable x64)" {
    $p = Get-RufusPortablePath
    Run-Rufus -Path $p
    break
  }

  # ---- Rufus (standard) ----
  "Download Rufus (standard x64)" {
    $p = Download-RufusStandard
    Write-Host "Saved to: $p"
    break
  }

  "Download + Run Rufus (standard x64)" {
    $p = Download-RufusStandard
    Run-Rufus -Path $p
    break
  }

  "Run downloaded Rufus (standard x64)" {
    $p = Get-RufusStandardPath
    Run-Rufus -Path $p
    break
  }

  "Open Rufus Releases" {
    Write-Host "Opening: $RufusReleasesPage"
    Start-Process $RufusReleasesPage
    break
  }

  "Delete downloaded Rufus" {
    $p1 = Get-RufusPortablePath
    $p2 = Get-RufusStandardPath
    $deleted = $false

    if (Test-Path $p1) { Remove-Item -LiteralPath $p1 -Force; Write-Host "Deleted: $p1"; $deleted = $true }
    if (Test-Path $p2) { Remove-Item -LiteralPath $p2 -Force; Write-Host "Deleted: $p2"; $deleted = $true }

    if (-not $deleted) { Write-Host "Nothing to delete (no Rufus files found in temp store)." }
    break
  }

  default {
    throw ("Unknown option: {0}" -f $Option)
  }
}
