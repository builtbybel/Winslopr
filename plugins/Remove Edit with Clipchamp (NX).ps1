# ================================================================================
# Plugin: Remove "Edit with Clipchamp"
# ================================================================================

[Commands]
Info=Removes "Edit with Clipchamp" context menu entry
Check=reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {8AB635F8-9A67-4698-AB99-784AD929F3B4}
Do=reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {8AB635F8-9A67-4698-AB99-784AD929F3B4} /t REG_SZ /d RemoveClipchampContext /f
Undo=reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {8AB635F8-9A67-4698-AB99-784AD929F3B4} /f

[Expect]
{8AB635F8-9A67-4698-AB99-784AD929F3B4}=RemoveClipchampContext

