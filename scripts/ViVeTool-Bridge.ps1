# Description: Manage ViVeTool and toggle/query feature IDs.
# Category: Post
# Options: Enable IDs;Disable IDs;Query IDs;Open GitHub Releases;Show Help
# Host: log
# Input: true
# InputPlaceholder: Enter comma-separated IDs or custom ViVeTool args
# PoweredBy: Albacore
# PoweredUrl: https://www.github.com/thebookisclosed


[CmdletBinding()]
param(
    [string]$Option,
    [string]$ArgsText
)

# --- Helpers -------------------------------------------------------------

function Get-BaseDir {
    try { return [AppDomain]::CurrentDomain.BaseDirectory } catch { return (Get-Location).Path }
}

function Resolve-ViVeTool {
    param([string]$base = (Get-BaseDir))

    # 1) Prefer expected folder: scripts\ViveTool\ViVeTool.exe
    $preferred = Join-Path $base "scripts\ViveTool\ViVeTool.exe"
    if (Test-Path $preferred) { 
        try { Unblock-File -Path $preferred -ErrorAction SilentlyContinue } catch {}
        return (Get-Item $preferred).FullName 
    }

    # 2) If folder exists, accept any *.exe inside it (in case filename differs)
    $folder = Join-Path $base "scripts\ViveTool"
    if (Test-Path $folder) {
        $any = Get-ChildItem -Path $folder -Filter "*.exe" -File -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($any) { 
            try { Unblock-File $any.FullName -ErrorAction SilentlyContinue } catch {}
            return $any.FullName 
        }
    }

    # 3) Environment override
    if ($env:VIVETOOL -and (Test-Path $env:VIVETOOL)) { return $env:VIVETOOL }

    # 4) PATH lookup
    try {
        $cmd = Get-Command ViVeTool.exe -ErrorAction SilentlyContinue
        if ($cmd -and (Test-Path $cmd.Path)) { return $cmd.Path }
    } catch {}

    # 5) Fallback search
    $roots = @(
        (Join-Path $base "scripts"),
        $PSScriptRoot,
        $base,
        (Get-Location).Path
    ) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique

    foreach ($r in $roots) {
        $hit = Get-ChildItem -Path $r -Recurse -File -Include "ViVeTool.exe","*ViVeTool*.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($hit) { return $hit.FullName }
    }

    return $null
}


function Ensure-ViVeTool {
    $exe = Resolve-ViVeTool
    if ($exe) { return $exe }
    throw "ViVeTool.exe not found. Put it under .\scripts or .\plugins (directly or in a subfolder), set VIVETOOL env var, or add it to PATH."
}

function Parse-Ids([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return @() }
    $s.Split(',') | ForEach-Object {
        $t = $_.Trim()
        if ($t -match '^\d+$') { [int]$t }
    } | Where-Object { $_ -ne $null }
}

# --- Main ----------------------------------------------------------------

switch ($Option) {

    'Open GitHub Releases' {
        Start-Process 'https://github.com/thebookisclosed/ViVe/releases' | Out-Null
        Write-Output "Opening https://github.com/thebookisclosed/ViVe/releases"
        break
    }

    'Show Help' {
@"
ViVeTool Helper — How it works

What this does:
- Lets you enable, disable, or query hidden Windows features using ViVeTool.
- Uses the text field for IDs or custom arguments.

Before you start:
- Place ViVeTool.exe under .\scripts or .\plugins (directly or inside a subfolder),
  OR set the VIVETOOL environment variable to the full path of ViVeTool.exe,
  OR ensure ViVeTool.exe is on PATH.

Options:
- Enable IDs     : Turns features ON. Provide comma-separated IDs in the text field.
- Disable IDs    : Turns features OFF. Provide comma-separated IDs in the text field.
- Query IDs      : Prints current status per ID.
- Open GitHub Releases : Opens the official releases page in your browser.
- Show Help      : Displays this guide.

Examples (Text field):
- 47205210,49221331,49381526
- 45624564
- For advanced users: select no specific action and pass raw ViVeTool arguments
  using 'Run' from your tool host (if supported), e.g.: /enable /id:45624564

Notes:
- Run as Administrator for system-wide changes.
- IDs must be numbers, separated by commas (no spaces required).
"@ | Write-Output
        break
    }

    'Enable IDs' {
        $exe = Ensure-ViVeTool
        $ids = Parse-Ids $ArgsText
        if ($ids.Count -eq 0) { throw "No valid IDs in Args." }
        $idArg = "/id:$($ids -join ',')"      # pass as single token
        Write-Output "Enabling: $($ids -join ',')"
        & $exe '/enable' $idArg
        break
    }

    'Disable IDs' {
        $exe = Ensure-ViVeTool
        $ids = Parse-Ids $ArgsText
        if ($ids.Count -eq 0) { throw "No valid IDs in Args." }
        $idArg = "/id:$($ids -join ',')"      # pass as single token
        Write-Output "Disabling: $($ids -join ',')"
        & $exe '/disable' $idArg
        break
    }

    'Query IDs' {
        $exe = Ensure-ViVeTool
        $ids = Parse-Ids $ArgsText
        if ($ids.Count -eq 0) { throw "No valid IDs in Args." }
        foreach ($id in $ids) {
            Write-Output "----- Query $id -----"
            & $exe '/query' "/id:$id"          # single token per ID
        }
        break
    }

    default {
        # Power user: pass raw arguments (e.g., "/enable /id:45624564")
        $exe = Ensure-ViVeTool
        if ([string]::IsNullOrWhiteSpace($ArgsText)) { throw "No Option selected and ArgsText is empty." }
        Write-Output "Running: $exe $ArgsText"
        & $exe $ArgsText
        break
    }
}
