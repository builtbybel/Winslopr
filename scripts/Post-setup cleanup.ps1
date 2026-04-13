# Post-setup cleanup (finish up)
# Category: Post
# Options: Disk Cleanup (recommended); Remove Windows.old; Component Store Cleanup (safe); Component Store Cleanup (aggressive); Clear Windows Update cache; Clear TEMP files

param([string]$choice)

switch ($choice) {
  "Disk Cleanup (recommended)" {
    $flagsPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches"
    $id = 1234
    Get-ChildItem $flagsPath | ForEach-Object {
      New-ItemProperty -Path $_.PSPath -Name ("StateFlags$id") -PropertyType DWord -Value 2 -Force -ErrorAction SilentlyContinue | Out-Null
    }
    Start-Process -FilePath "cleanmgr.exe" -ArgumentList "/sagerun:$id" -Wait
    Write-Output "Disk Cleanup done."
  }
  "Remove Windows.old" {
    try {
      if (Test-Path "C:\Windows.old") {
        takeown /F C:\Windows.old /A /R /D Y | Out-Null
        icacls C:\Windows.old /grant administrators:F /T /C | Out-Null
        Remove-Item "C:\Windows.old" -Recurse -Force -ErrorAction Stop
        Write-Output "Windows.old removed."
      } else { Write-Output "Windows.old not found." }
    } catch { Write-Output "Failed to remove Windows.old: $($_.Exception.Message)" }
  }
  "Component Store Cleanup (safe)" {
    Start-Process -FilePath "Dism.exe" -ArgumentList "/online /Cleanup-Image /StartComponentCleanup" -Wait
    Write-Output "Component store cleanup (safe) done."
  }
  "Component Store Cleanup (aggressive)" {
    Start-Process -FilePath "Dism.exe" -ArgumentList "/online /Cleanup-Image /StartComponentCleanup /ResetBase" -Wait
    Write-Output "Component store cleanup (aggressive) done. (Cannot uninstall updates)"
  }
  "Clear Windows Update cache" {
    Start-Process -FilePath "cmd.exe" -ArgumentList "/c net stop wuauserv & net stop bits" -Verb runas -Wait
    if (Test-Path "C:\Windows\SoftwareDistribution\Download") {
      Remove-Item "C:\Windows\SoftwareDistribution\Download\*" -Recurse -Force -ErrorAction SilentlyContinue
    }
    Start-Process -FilePath "cmd.exe" -ArgumentList "/c net start bits & net start wuauserv" -Verb runas -Wait
    Write-Output "Windows Update cache cleared."
  }
  "Clear TEMP files" {
    Remove-Item "$env:TEMP\*" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Output "TEMP cleared."
  }
  default { Write-Output "Unknown option: $choice" }
}
