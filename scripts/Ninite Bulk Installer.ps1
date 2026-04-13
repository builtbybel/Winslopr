# Description: Install multiple apps via Ninite (downloads to Desktop).
# Category: Post
# Options: Install packages;Preview URL;Open Ninite Website;Show Help
# Host: log
# Input: true
# InputPlaceholder: Enter app slugs or names (comma/space/newline). Example: chrome 7zip vlc git

[CmdletBinding()]
param(
  [string]$Option,
  [string]$ArgsText
)

# ------------------------ Helpers ------------------------

function Get-DesktopPath {
  try { return [Environment]::GetFolderPath('Desktop') } catch { return (Join-Path $env:USERPROFILE 'Desktop') }
}

function Is-Admin {
  try {
    $id = [Security.Principal.WindowsIdentity]::GetCurrent()
    $pr = New-Object Security.Principal.WindowsPrincipal $id
    return $pr.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
  } catch { return $false }
}

# Map common names to Ninite slugs (extend freely). Slugs must match ninite.com.
$SlugMap = @{
  "7zip"="7zip"; "winrar"="winrar"; "chrome"="chrome"; "firefox"="firefox"; "edge"="edge"; "opera"="opera"
  "vlc"="vlc"; "spotify"="spotify"; "skype"="skype"; "teams"="teams"; "discord"="discord"
  "notepad++"="notepadplusplus"; "notepadplusplus"="notepadplusplus"; "sumatrapdf"="sumatrapdf"; "irfanview"="irfanview"
  "git"="git"; "putty"="putty"; "python"="python"; "nodejs"="nodejs"; "java8"="jre8"; "dotnet"="dotnet"
  "visual studio code"="vscode"; "vscode"="vscode"
}

function Normalize-ToSlug([string]$token) {
  if ([string]::IsNullOrWhiteSpace($token)) { return $null }
  $k = $token.Trim().ToLower()
  if ($SlugMap.ContainsKey($k)) { return $SlugMap[$k] }
  # Allow raw slugs; strip spaces (users sometimes paste names with spaces).
  return ($k -replace '\s+','')
}

function Parse-Packages([string]$s) {
  if ([string]::IsNullOrWhiteSpace($s)) { return @() }
  $parts = $s -split "[\r\n,; ]+" | Where-Object { $_ -and $_.Trim().Length -gt 0 } | ForEach-Object { $_.Trim() }
  $slugs = @()
  foreach ($p in $parts) {
    $slug = Normalize-ToSlug $p
    if ($slug) { $slugs += $slug }
  }
  # Deduplicate while preserving order
  $seen = @{}
  $out = @()
  foreach ($x in $slugs) { if (-not $seen.ContainsKey($x)) { $seen[$x]=$true; $out += $x } }
  return $out
}

function Build-NiniteUrl($apps) {
  if ($apps -eq $null -or $apps.Count -eq 0) { throw "No packages provided." }
  $pathPart = ($apps -join '-')
  return "https://ninite.com/$pathPart/ninite.exe"
}

function Open-Url($url) {
  try { Start-Process $url | Out-Null } catch { Write-Output "Open manually: $url" }
}

# Lightweight HEAD check; if HEAD is blocked by CDN, return true and let GET decide.
function Test-Url-Exists($url) {
  try {
    $resp = Invoke-WebRequest -Uri $url -Method Head -UseBasicParsing -ErrorAction Stop
    return ($resp.StatusCode -ge 200 -and $resp.StatusCode -lt 300)
  } catch { return $true }
}

# Our launcher tries this: elevated > non-elevated >  then explorer fallback.
function Try-StartExe([string]$path, [string]$args, [bool]$elevate, [string]$note) {
  try {
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $path
    $psi.Arguments = $args
    $psi.WorkingDirectory = (Split-Path -Parent $path)
    $psi.UseShellExecute = $true
    $psi.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Normal
    if ($elevate) { $psi.Verb = 'runas' }  # triggers UAC
    Write-Output "Launching ($note): $path $args"
    $p = [System.Diagnostics.Process]::Start($psi)
    if ($p) { $p.WaitForExit(); return $p.ExitCode }
  } catch {
    Write-Output "Launch failed ($note): $($_.Exception.Message)"
  }
  return $null
}

# ------------------------- Main --------------------------

switch ($Option) {

  'Open Ninite Website' {
    Open-Url 'https://ninite.com/'
    Write-Output 'Opening https://ninite.com/'
    break
  }

  'Preview URL' {
    $apps = Parse-Packages $ArgsText
    if ($apps.Count -eq 0) { throw "No packages provided. Example: chrome 7zip vlc" }
    $url = Build-NiniteUrl $apps
    Write-Output "Preview: $url"
    Open-Url $url
    break
  }

  'Show Help' {
@"
Ninite Installer — How to use

- Enter app names or slugs in the text field (comma/space/newline separated).
  Examples:  chrome 7zip vlc   |   notepad++ git vscode
- The script maps common names to Ninite slugs and builds:
  https://ninite.com/<slug1>-<slug2>-.../ninite.exe
- The installer is downloaded to your Desktop and launched (visible window).
  If elevation is required, you'll see a UAC prompt.

Tips:
- If launch fails silently, SmartScreen or a policy may be blocking headless starts.
  This script shows the window and also tries an Explorer fallback (like double-click).
- If exit code = -2147467259, it often means invalid URL (404), SmartScreen block, or denied elevation.
- Full app list/slugs: https://ninite.com/
"@ | Write-Output
    break
  }

  'Install packages' {
    # 1) Build slugs and URL
    $apps = Parse-Packages $ArgsText
    if ($apps.Count -eq 0) { throw "No packages provided. Example: chrome 7zip vlc" }
    $url = Build-NiniteUrl $apps
    Write-Output "URL: $url"
    try { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 } catch {}

    # 2) Download to Desktop with a predictable, readable name
    $desktop  = Get-DesktopPath
    $fileName = 'Ninite-' + ($apps -join '-') + '.exe'   # e.g., Ninite-chrome-7zip-vlc.exe
    $outFile  = Join-Path $desktop $fileName

    # Optional pre-check (HEAD)
    if (-not (Test-Url-Exists $url)) {
      throw "Ninite URL seems invalid. Check app slugs or use 'Preview URL'."
    }

    Write-Output "Downloading to: $outFile"
    try {
      Invoke-WebRequest -Uri $url -OutFile $outFile -UseBasicParsing -ErrorAction Stop
    } catch {
      throw "Download failed: $($_.Exception.Message)"
    }

    if (-not (Test-Path $outFile)) { throw "Download failed — file not found at: $outFile" }

    # 3) Unblock & quick sanity check
    try { Unblock-File -Path $outFile -ErrorAction SilentlyContinue } catch {}
    $len = (Get-Item $outFile).Length
    Write-Output ("File size: {0:N0} bytes" -f $len)
    if ($len -lt 100000) { Write-Output "Warning: file seems small (maybe 404 HTML). Try 'Preview URL' first." }

    # 4) Try to run (elevated → non-elevated). Keep window visible to surface prompts.
    $code = Try-StartExe $outFile "/silent" $true  "elevated"
    if ($code -eq $null) {
      $code = Try-StartExe $outFile "/silent" $false "non-elevated"
    }

    # 5) If still no process exit code (launch failed), try the most recent Ninite-*.exe on Desktop
    if ($code -eq $null) {
      $latest = Get-ChildItem -Path $desktop -Filter 'Ninite-*.exe' -File -ErrorAction SilentlyContinue |
                Sort-Object LastWriteTime -Descending | Select-Object -First 1
      if ($latest -and ($latest.FullName -ne $outFile)) {
        Write-Output "Trying latest found EXE: $($latest.FullName)"
        $code = Try-StartExe $latest.FullName "/silent" $true  "elevated(latest)"
        if ($code -eq $null) { $code = Try-StartExe $latest.FullName "/silent" $false "non-elevated(latest)" }
      }
    }

    # 6) Final fallback: open via Explorer (like a user double-click). This will show SmartScreen/UAC visibly.
    if ($code -eq $null) {
      try {
        Write-Output "Fallback: opening via explorer.exe"
        Start-Process explorer.exe "`"$outFile`""
        Write-Output "If nothing appears, double-click the EXE once to clear SmartScreen, then re-run."
      } catch {
        Write-Output "Explorer fallback failed: $($_.Exception.Message)"
      }
    } else {
      Write-Output "Ninite finished with exit code: $code"
      if ($code -eq -2147467259) {
        Write-Output "Hint: -2147467259 often indicates invalid URL (404), SmartScreen block, or denied elevation."
      }
    }

    break
  }

  default {
    throw "Unknown option. Use: Install packages; Preview URL; Open Ninite Website; Show Help"
  }
}
