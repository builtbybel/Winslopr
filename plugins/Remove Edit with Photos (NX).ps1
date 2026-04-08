# ================================================================================
# Plugin: Remove "Edit with Photos"
# ================================================================================

[Commands]
Info=Removes "Edit with Photos" context menu entry
Check=reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {BFE0E2A4-C70C-4AD7-AC3D-10D1ECEBB5B4}
Do=reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {BFE0E2A4-C70C-4AD7-AC3D-10D1ECEBB5B4} /t REG_SZ /d RemovePhotosContext /f
Undo=reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {BFE0E2A4-C70C-4AD7-AC3D-10D1ECEBB5B4} /f

[Expect]
{BFE0E2A4-C70C-4AD7-AC3D-10D1ECEBB5B4}=RemovePhotosContext
