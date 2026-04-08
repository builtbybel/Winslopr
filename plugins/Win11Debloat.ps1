# Win11Debloat Launcher.ps1
[Commands]
Info=Launches Raphire's Win11Debloat using the official remote bootstrap.
Check=echo Ready
Do=powershell -NoProfile -ExecutionPolicy Bypass -Command "& ([scriptblock]::Create((irm 'https://debloat.raphi.re/')))"
Undo=echo No undo available