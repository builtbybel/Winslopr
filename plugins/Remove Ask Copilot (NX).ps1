# ================================================================================
# Plugin: Remove "Ask Copilot"
# ================================================================================

[Commands]
Info=Removes "Ask Copilot" context menu entry
Check=reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CB3B0003-8088-4EDE-8769-8B354AB2FF8C}
Do=reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CB3B0003-8088-4EDE-8769-8B354AB2FF8C} /t REG_SZ /d RemoveCopilotContext /f
Undo=reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked" /v {CB3B0003-8088-4EDE-8769-8B354AB2FF8C} /f

[Expect]
{CB3B0003-8088-4EDE-8769-8B354AB2FF8C}=RemoveCopilotContext
