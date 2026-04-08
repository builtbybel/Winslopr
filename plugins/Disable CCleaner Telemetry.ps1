[Commands]
Info=Disables CCleaner telemetry, monitoring, update checks, and promotional options.
Check=reg query "HKCU\Software\Piriform\CCleaner" /v Monitoring && reg query "HKCU\Software\Piriform\CCleaner" /v HelpImproveCCleaner && reg query "HKCU\Software\Piriform\CCleaner" /v SystemMonitoring && reg query "HKCU\Software\Piriform\CCleaner" /v UpdateAuto && reg query "HKCU\Software\Piriform\CCleaner" /v UpdateCheck && reg query "HKCU\Software\Piriform\CCleaner" /v CheckTrialOffer && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)HealthCheck" && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickClean" && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickCleanIpm" && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)GetIpmForTrial" && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdater" && reg query "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdaterIpm"
Do=reg add "HKCU\Software\Piriform\CCleaner" /v Monitoring /t REG_DWORD /d 0 /f && reg add "HKCU\Software\Piriform\CCleaner" /v HelpImproveCCleaner /t REG_DWORD /d 0 /f && reg add "HKCU\Software\Piriform\CCleaner" /v SystemMonitoring /t REG_DWORD /d 0 /f && reg add "HKCU\Software\Piriform\CCleaner" /v UpdateAuto /t REG_DWORD /d 0 /f && reg add "HKCU\Software\Piriform\CCleaner" /v UpdateCheck /t REG_DWORD /d 0 /f && reg add "HKCU\Software\Piriform\CCleaner" /v CheckTrialOffer /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)HealthCheck" /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickClean" /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickCleanIpm" /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)GetIpmForTrial" /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdater" /t REG_DWORD /d 0 /f && reg add "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdaterIpm" /t REG_DWORD /d 0 /f
Undo=reg delete "HKCU\Software\Piriform\CCleaner" /v Monitoring /f && reg delete "HKCU\Software\Piriform\CCleaner" /v HelpImproveCCleaner /f && reg delete "HKCU\Software\Piriform\CCleaner" /v SystemMonitoring /f && reg delete "HKCU\Software\Piriform\CCleaner" /v UpdateAuto /f && reg delete "HKCU\Software\Piriform\CCleaner" /v UpdateCheck /f && reg delete "HKCU\Software\Piriform\CCleaner" /v CheckTrialOffer /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)HealthCheck" /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickClean" /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)QuickCleanIpm" /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)GetIpmForTrial" /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdater" /f && reg delete "HKLM\Software\Piriform\CCleaner" /v "(Cfg)SoftwareUpdaterIpm" /f

[Expect]
Monitoring=0x0
HelpImproveCCleaner=0x0
SystemMonitoring=0x0
UpdateAuto=0x0
UpdateCheck=0x0
CheckTrialOffer=0x0
(Cfg)HealthCheck=0x0
(Cfg)QuickClean=0x0
(Cfg)QuickCleanIpm=0x0
(Cfg)GetIpmForTrial=0x0
(Cfg)SoftwareUpdater=0x0
(Cfg)SoftwareUpdaterIpm=0x0