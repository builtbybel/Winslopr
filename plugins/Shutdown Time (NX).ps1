# ================================================================================
# ⚡ FastShutdown
# This plugin speeds up the Windows shutdown process by reducing wait times
# for background and non-responsive applications to close.
#
# ✅ Ideal for power users who want a faster shutdown or reboot experience.
# 🔄 Undo restores default wait timings for safer app closure.
# ================================================================================

[Commands]
Info=Decreases wait time during shutdown and kills non-responsive apps faster
Check=reg query "HKCU\Control Panel\Desktop" /v WaitToKillAppTimeout && reg query "HKCU\Control Panel\Desktop" /v HungAppTimeout
Do=reg add "HKCU\Control Panel\Desktop" /v WaitToKillAppTimeout /t REG_SZ /d 2000 /f && reg add "HKCU\Control Panel\Desktop" /v HungAppTimeout /t REG_SZ /d 1000 /f
Undo=reg add "HKCU\Control Panel\Desktop" /v WaitToKillAppTimeout /t REG_SZ /d 5000 /f && reg add "HKCU\Control Panel\Desktop" /v HungAppTimeout /t REG_SZ /d 5000 /f

[Expect]
WaitToKillAppTimeout=2000
HungAppTimeout=1000
