# Description: Windows 11 repair/reset wizard (built-in).
# Category: Tool
# Host: embedded

param([string]$Option, [string]$ArgsText)  

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

function Confirm-Action([string]$title, [string]$message) {
  $res = [System.Windows.Forms.MessageBox]::Show(
    $message,
    $title,
    [System.Windows.Forms.MessageBoxButtons]::YesNo,
    [System.Windows.Forms.MessageBoxIcon]::Warning
  )
  return ($res -eq [System.Windows.Forms.DialogResult]::Yes)
}

function Run-Elevated([string]$file, [string]$args) {
  # UAC prompt (Admin needed for SFC/DISM/Shutdown)
  Start-Process -FilePath $file -ArgumentList $args -Verb RunAs
}

function Start-SystemResetWizard {
  # Try factoryreset switch; if not supported on that build, just open systemreset.exe
  try {
    Start-Process -FilePath "systemreset.exe" -ArgumentList "-factoryreset" -WindowStyle Normal
  } catch {
   # Fallback for newer Win11 builds (incl. cases where systemreset.exe is missing)
  Start-Process "ms-settings:recovery"
  }
}

# ---------------- UI ----------------
$form = New-Object System.Windows.Forms.Form
$form.Text = "Winslop – Win11 is busted (Repair Wizard)"
$form.StartPosition = 'CenterParent'
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false
$form.ShowInTaskbar = $false
$form.AutoSize = $true
$form.AutoSizeMode = 'GrowAndShrink'
$form.Padding = New-Object System.Windows.Forms.Padding(12)

$root = New-Object System.Windows.Forms.TableLayoutPanel
$root.ColumnCount = 1
$root.RowCount = 1
$root.AutoSize = $true
$root.Dock = 'Fill'

$lblTop = New-Object System.Windows.Forms.Label
$lblTop.AutoSize = $true
$lblTop.MaximumSize = New-Object System.Drawing.Size(560, 0)
$lblTop.Text = "Pick what you want to do. This uses Windows built-in tools (no downloads). " +
               "Some actions require Administrator and may restart the PC."

# Radio options
$rbSettings = New-Object System.Windows.Forms.RadioButton
$rbSettings.Text = "Open Recovery Settings (ms-settings:recovery)"
$rbSettings.AutoSize = $true
$rbSettings.Checked = $true

$rbReset = New-Object System.Windows.Forms.RadioButton
$rbReset.Text = "Open 'Reset this PC' wizard (systemreset.exe)"
$rbReset.AutoSize = $true

$rbWinRE = New-Object System.Windows.Forms.RadioButton
$rbWinRE.Text = "Reboot into Advanced Startup (WinRE)"
$rbWinRE.AutoSize = $true

$rbSfc = New-Object System.Windows.Forms.RadioButton
$rbSfc.Text = "Repair system files (SFC /scannow)  [Admin]"
$rbSfc.AutoSize = $true

$rbDism = New-Object System.Windows.Forms.RadioButton
$rbDism.Text = "Repair component store (DISM RestoreHealth)  [Admin]"
$rbDism.AutoSize = $true

# Details box
$txtInfo = New-Object System.Windows.Forms.TextBox
$txtInfo.Multiline = $true
$txtInfo.ReadOnly = $true
$txtInfo.ScrollBars = 'Vertical'
$txtInfo.WordWrap = $true
$txtInfo.Width = 560
$txtInfo.Height = 170

function Set-InfoText {
  if ($rbSettings.Checked) {
    $txtInfo.Text =
      "Opens Windows Recovery settings.`r`n" +
      "Useful to access Advanced startup, Reset options and recovery features."
  }
  elseif ($rbReset.Checked) {
    $txtInfo.Text =
      "Opens the built-in Reset wizard (Reset this PC).`r`n`r`n" +
      "Inside the wizard you choose:`r`n" +
      "- Keep my files (removes apps/settings, keeps personal files)`r`n" +
      "- Remove everything (wipes apps/files/settings)`r`n" +
      "On some builds you can pick Cloud download vs Local reinstall.`r`n`r`n" +
      "Note: Windows doesn't provide supported CLI switches to preselect those choices."
  }
  elseif ($rbWinRE.Checked) {
    $txtInfo.Text =
      "Reboots immediately into Advanced Startup (WinRE).`r`n`r`n" +
      "From there you can use:`r`n" +
      "- Startup Repair`r`n" +
      "- System Restore`r`n" +
      "- Uninstall updates`r`n" +
      "- UEFI firmware settings`r`n`r`n" +
      "This will restart the PC right away."
  }
  elseif ($rbSfc.Checked) {
    $txtInfo.Text =
      "Runs System File Checker:`r`n" +
      "  sfc /scannow`r`n`r`n" +
      "Checks and repairs protected Windows system files.`r`n" +
      "Can take 5–30+ minutes. Requires Administrator."
  }
  elseif ($rbDism.Checked) {
    $txtInfo.Text =
      "Runs DISM component store repair:`r`n" +
      "  DISM /Online /Cleanup-Image /RestoreHealth`r`n`r`n" +
      "Fixes Windows component store corruption (often helps when SFC can't).`r`n" +
      "May take a while and can use Windows Update. Requires Administrator."
  }
}

$handler = [System.EventHandler]{ Set-InfoText }
$rbSettings.add_CheckedChanged($handler)
$rbReset.add_CheckedChanged($handler)
$rbWinRE.add_CheckedChanged($handler)
$rbSfc.add_CheckedChanged($handler)
$rbDism.add_CheckedChanged($handler)

Set-InfoText

# Buttons
$pnlButtons = New-Object System.Windows.Forms.FlowLayoutPanel
$pnlButtons.FlowDirection = 'RightToLeft'
$pnlButtons.AutoSize = $true
$pnlButtons.Margin = New-Object System.Windows.Forms.Padding(0, 12, 0, 0)

$btnRun = New-Object System.Windows.Forms.Button
$btnRun.Text = "Run"
$btnRun.AutoSize = $true

$btnCancel = New-Object System.Windows.Forms.Button
$btnCancel.Text = "Cancel"
$btnCancel.AutoSize = $true
$btnCancel.Margin = New-Object System.Windows.Forms.Padding(6, 0, 0, 0)
$btnCancel.DialogResult = [System.Windows.Forms.DialogResult]::Cancel

$pnlButtons.Controls.Add($btnRun) | Out-Null
$pnlButtons.Controls.Add($btnCancel) | Out-Null

$btnRun.Add_Click({
  try {
    if ($rbSettings.Checked) {
      Start-Process "ms-settings:recovery"
      $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
      $form.Close()
      return
    }

    if ($rbReset.Checked) {
      $ok = Confirm-Action "Reset this PC" (
        "Open the Reset wizard now?`r`n`r`n" +
        "You will choose Keep/Remove + Cloud/Local inside the wizard. " +
        "Depending on your choice, apps/data can be removed."
      )
      if (-not $ok) { return }
      Start-SystemResetWizard
      $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
      $form.Close()
      return
    }

    if ($rbWinRE.Checked) {
      $ok = Confirm-Action "Reboot to WinRE" "Reboot now into Advanced Startup (WinRE)?`r`n`r`nYour PC will restart immediately."
      if (-not $ok) { return }
      Run-Elevated "shutdown.exe" "/r /o /t 0 /f"
      $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
      $form.Close()
      return
    }

    if ($rbSfc.Checked) {
      $ok = Confirm-Action "Run SFC" "Run: sfc /scannow ?`r`n`r`nRequires Administrator. A console window will open."
      if (-not $ok) { return }
      Run-Elevated "cmd.exe" "/c sfc /scannow"
      $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
      $form.Close()
      return
    }

    if ($rbDism.Checked) {
      $ok = Confirm-Action "Run DISM" "Run: DISM /Online /Cleanup-Image /RestoreHealth ?`r`n`r`nRequires Administrator. A console window will open."
      if (-not $ok) { return }
      Run-Elevated "cmd.exe" "/c DISM /Online /Cleanup-Image /RestoreHealth"
      $form.DialogResult = [System.Windows.Forms.DialogResult]::OK
      $form.Close()
      return
    }
  } catch {
    [System.Windows.Forms.MessageBox]::Show(
      $_.Exception.Message,
      "Winslop – Error",
      [System.Windows.Forms.MessageBoxButtons]::OK,
      [System.Windows.Forms.MessageBoxIcon]::Error
    ) | Out-Null
  }
})

# Layout
$root.Controls.Add($lblTop)     | Out-Null
$root.Controls.Add($rbSettings) | Out-Null
$root.Controls.Add($rbReset)    | Out-Null
$root.Controls.Add($rbWinRE)    | Out-Null
$root.Controls.Add($rbSfc)      | Out-Null
$root.Controls.Add($rbDism)     | Out-Null
$root.Controls.Add($txtInfo)    | Out-Null
$root.Controls.Add($pnlButtons) | Out-Null

$form.Controls.Add($root) | Out-Null
$form.AcceptButton = $btnRun
$form.CancelButton = $btnCancel

[void]$form.ShowDialog()
Write-Output "Done."
