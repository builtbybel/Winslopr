# ================================================================================
# Plugin: Remove "Edit with Notepad"
# ================================================================================

[Commands]
Info=Removes "Edit with Notepad" context menu entry
Check=reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CA6CC9F1-867A-481E-951E-A28C5E4F01EA}
Do=reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CA6CC9F1-867A-481E-951E-A28C5E4F01EA} /t REG_SZ /d RemoveNotepadContext /f
Undo=reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CA6CC9F1-867A-481E-951E-A28C5E4F01EA} /f

[Expect]
{CA6CC9F1-867A-481E-951E-A28C5E4F01EA}=RemoveNotepadContext
