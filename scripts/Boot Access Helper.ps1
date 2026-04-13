# Description: Shows common Boot Menu and BIOS/UEFI keys based on detected manufacturer (info-only).
# Category: Tool
# Host: embedded
# Options: Show info dialog; Print to output (log)


param(
  [string]$Option,
  [string]$ArgsText
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function Get-SystemVendor {
  try {
    $cs = Get-CimInstance -ClassName Win32_ComputerSystem

    $m  = ""
    $mo = ""

    if ($cs -and $cs.Manufacturer -ne $null) { $m  = [string]$cs.Manufacturer }
    if ($cs -and $cs.Model        -ne $null) { $mo = [string]$cs.Model }

    $m  = $m.Trim()
    $mo = $mo.Trim()

    if ([string]::IsNullOrWhiteSpace($m))  { $m  = $null }
    if ([string]::IsNullOrWhiteSpace($mo)) { $mo = $null }

    return @{ Manufacturer = $m; Model = $mo }
  } catch {
    return @{ Manufacturer = $null; Model = $null }
  }
}

function Get-VendorMap {
  # Array of hashtables (super compatible)
  return @(
    @{ Vendor="Acer";      BootMenu="F12";                    BiosSetup="F2" },
    @{ Vendor="ASUS";      BootMenu="Esc or F8";              BiosSetup="Del or F2" },
    @{ Vendor="Dell";      BootMenu="F12";                    BiosSetup="F2" },
    @{ Vendor="HP";        BootMenu="Esc or F9";              BiosSetup="Esc or F10" },
    @{ Vendor="Lenovo";    BootMenu="F12 (Fn+F12)";           BiosSetup="F1 or F2" },
    @{ Vendor="MSI";       BootMenu="F11";                    BiosSetup="Del" },
    @{ Vendor="Gigabyte";  BootMenu="F12";                    BiosSetup="Del" },
    @{ Vendor="ASRock";    BootMenu="F11";                    BiosSetup="F2 or Del" },
    @{ Vendor="Toshiba";   BootMenu="F12";                    BiosSetup="F2" },
    @{ Vendor="Sony";      BootMenu="F11 or Assist";          BiosSetup="F2" },
    @{ Vendor="Samsung";   BootMenu="Esc or F12";             BiosSetup="F2" },
    @{ Vendor="Microsoft"; BootMenu="Vol-Down + Power (Surface)"; BiosSetup="Vol-Up + Power (UEFI)" }
  )
}

function Build-Content {
  $map = Get-VendorMap
  $sys = Get-SystemVendor
  $manufacturer = $sys.Manufacturer
  $model = $sys.Model

  $sb = New-Object System.Text.StringBuilder
  [void]$sb.AppendLine("Boot Menu & BIOS/UEFI keys")
  [void]$sb.AppendLine("---------------------------")

  if ($manufacturer -or $model) {
    $m  = $manufacturer; if (-not $m)  { $m  = "(unknown)" }
    $mo = $model;        if (-not $mo) { $mo = "(unknown)" }
    [void]$sb.AppendLine(("Detected: {0} {1}" -f $m, $mo))
  } else {
    [void]$sb.AppendLine("Detected: (unknown manufacturer/model)")
  }
  [void]$sb.AppendLine()

  # find best match
  $primary = $null
  if ($manufacturer) {
    $manLower = $manufacturer.ToLower()
    foreach ($x in $map) {
      if ($manLower.Contains(($x["Vendor"]).ToLower())) { $primary = $x; break }
    }
  }

  if ($primary) {
    [void]$sb.AppendLine(("★ Recommended for your device ({0}):" -f $primary["Vendor"]))
    [void]$sb.AppendLine(("   Boot Menu: {0}" -f $primary["BootMenu"]))
    [void]$sb.AppendLine(("   BIOS/UEFI: {0}" -f $primary["BiosSetup"]))
    [void]$sb.AppendLine()
  }

  [void]$sb.AppendLine("Common keys by vendor:")
  [void]$sb.AppendLine("Vendor        | Boot Menu              | BIOS/UEFI")
  [void]$sb.AppendLine("--------------+------------------------+----------------")
  foreach ($entry in $map) {
    $v  = ([string]$entry["Vendor"]).PadRight(12)
    $bm = ([string]$entry["BootMenu"]).PadRight(22)
    [void]$sb.AppendLine(("{0}| {1}| {2}" -f $v, $bm, $entry["BiosSetup"]))
  }

  [void]$sb.AppendLine()
  [void]$sb.AppendLine("Tips:")
  [void]$sb.AppendLine("- Press and hold the key right after power-on, before the Windows logo.")
  [void]$sb.AppendLine("- On some laptops you may need Fn with the function key (e.g., Fn+F12).")
  [void]$sb.AppendLine("- If you cannot reach the menu, open Recovery → Advanced startup → UEFI Firmware Settings.")
  [void]$sb.AppendLine("- This tool cannot truly 'read' boot keys from BIOS; it matches common keys by vendor.")
  return $sb.ToString()
}

function Show-Dialog([string]$text) {
  Add-Type -AssemblyName System.Windows.Forms
  Add-Type -AssemblyName System.Drawing

  $form = New-Object System.Windows.Forms.Form
  $form.Text = "Boot Menu keys (info)"
  $form.StartPosition = 'CenterParent'
  $form.FormBorderStyle = 'FixedDialog'
  $form.MaximizeBox = $false
  $form.MinimizeBox = $false
  $form.ShowInTaskbar = $false
  $form.AutoSize = $true
  $form.AutoSizeMode = 'GrowAndShrink'
  $form.Padding = New-Object System.Windows.Forms.Padding(12)

  $txt = New-Object System.Windows.Forms.TextBox
  $txt.Multiline = $true
  $txt.ReadOnly = $true
  $txt.ScrollBars = 'Vertical'
  $txt.WordWrap = $false
  $txt.Width = 560
  $txt.Height = 380
  $txt.Text = $text

  $btnCopy = New-Object System.Windows.Forms.Button
  $btnCopy.Text = "Copy to clipboard"
  $btnCopy.AutoSize = $true
  $btnCopy.Add_Click({ try { [System.Windows.Forms.Clipboard]::SetText($txt.Text) } catch { } })

  $btnRecovery = New-Object System.Windows.Forms.Button
  $btnRecovery.Text = "Open Recovery settings"
  $btnRecovery.AutoSize = $true
  $btnRecovery.Add_Click({ try { Start-Process "ms-settings:recovery" } catch { } })

  $btnClose = New-Object System.Windows.Forms.Button
  $btnClose.Text = "Close"
  $btnClose.AutoSize = $true
  $btnClose.DialogResult = [System.Windows.Forms.DialogResult]::OK

  $panel = New-Object System.Windows.Forms.FlowLayoutPanel
  $panel.FlowDirection = 'RightToLeft'
  $panel.AutoSize = $true
  $panel.Dock = 'Fill'
  $panel.Margin = New-Object System.Windows.Forms.Padding(0,8,0,0)
  [void]$panel.Controls.Add($btnClose)
  [void]$panel.Controls.Add($btnRecovery)
  [void]$panel.Controls.Add($btnCopy)

  $root = New-Object System.Windows.Forms.TableLayoutPanel
  $root.ColumnCount = 1
  $root.AutoSize = $true
  $root.Dock = 'Fill'
  [void]$root.Controls.Add($txt)
  [void]$root.Controls.Add($panel)

  [void]$form.Controls.Add($root)
  $form.AcceptButton = $btnClose
  $form.CancelButton = $btnClose

  [void]$form.ShowDialog()
}

if ([string]::IsNullOrWhiteSpace($Option)) { $Option = 'Show info dialog' }

$content = Build-Content

switch ($Option) {
  'Show info dialog'   { Show-Dialog $content; Write-Output "Closed info dialog." }
  'Print to output'    { Write-Output $content }
  default              { throw ("Unknown option: {0}" -f $Option) }
}
