[Commands]
Info=Disables NVIDIA telemetry-related registry values and scheduled telemetry tasks.
Check=reg query "HKLM\SOFTWARE\NVIDIA Corporation\NvControlPanel2\Client" /v OptInOrOutPreference && reg query "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID44231 && reg query "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID64640 && reg query "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID66610 && reg query "HKLM\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\Startup" /v SendTelemetryData
Do=reg add "HKLM\SOFTWARE\NVIDIA Corporation\NvControlPanel2\Client" /v OptInOrOutPreference /t REG_DWORD /d 0 /f && reg add "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID44231 /t REG_DWORD /d 0 /f && reg add "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID64640 /t REG_DWORD /d 0 /f && reg add "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID66610 /t REG_DWORD /d 0 /f && reg add "HKLM\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\Startup" /v SendTelemetryData /t REG_DWORD /d 0 /f && schtasks /Change /TN "NvTmMon_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Disable && schtasks /Change /TN "NvTmRep_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Disable && schtasks /Change /TN "NvTmRepOnLogon_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Disable
Undo=reg delete "HKLM\SOFTWARE\NVIDIA Corporation\NvControlPanel2\Client" /v OptInOrOutPreference /f && reg delete "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID44231 /f && reg delete "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID64640 /f && reg delete "HKLM\SOFTWARE\NVIDIA Corporation\Global\FTS" /v EnableRID66610 /f && reg delete "HKLM\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\Startup" /v SendTelemetryData /f && schtasks /Change /TN "NvTmMon_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Enable && schtasks /Change /TN "NvTmRep_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Enable && schtasks /Change /TN "NvTmRepOnLogon_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}" /Enable

[Expect]
OptInOrOutPreference=0x0
EnableRID44231=0x0
EnableRID64640=0x0
EnableRID66610=0x0
SendTelemetryData=0x0