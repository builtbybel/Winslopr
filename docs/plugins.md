# Plugins

Winslop plugins are **PowerShell scripts (`.ps1`) with an INI-style header**.
Winslop reads the header to decide what to run (Check / Do / Undo) and may validate results via [Expect].

Plugins can:
- modify registry / services / files via commands
- run built-in Windows tools (e.g. `cleanmgr`, `powercfg`, `dism`, `winget`)
- launch external utilities (if present)
- optionally execute extra PowerShell logic after the INI sections

> Built-in Windows toggles are documented in: [features.md](features.md)  
> Extensions / external add-ons are documented in: [extensions.md](extensions.md)

---

## Plugin file format

A plugin is a `.ps1` file that may contain:

- `[Commands]` (required)
- `[Expect]` (optional)

Everything outside these sections is treated as normal PowerShell script content.

---
## Recommended plugin structure

Keep plugins:
- transparent (explain what they do)
- reversible (provide undo if possible)
- quiet (no unnecessary output)
- safe (avoid remote downloads unless required)

---

## Basic execution model 

The demo pack uses a simple rule-driven model:

1. **Check** is executed first  
   The tool evaluates the output against **Expect**. 
2. If a fix is triggered, **Do** is executed.
3. Optionally, an **Undo** command can be provided.

---

## Naming and Help anchors

Winslop’s help system can link directly into GitHub docs by using the plugin title as an anchor.
That means:

- The plugin **display title** should match the markdown heading in `features.md` (plugins section)
- Anchors are generated like GitHub:
  - lowercase
  - spaces become `-`
  - apostrophes are removed (`Don't` → `dont`)

Example:
- Plugin title: `Don't Show Sponsored links in new tab page`
- Anchor: `#dont-show-sponsored-links-in-new-tab-page`

---

## [Commands]

`[Commands]` defines the metadata and the actions Winslop can run.

Supported keys:
- `Info` — shown in the UI as plugin description
- `Check` — executed first to read current state (its output may be validated)
- `Do` — executed when the user applies the action/fix
- `Undo` — executed to revert changes (if supported)

Notes:
- You can chain multiple commands using `&&`
- Output from all commands is logged
- `Check` can include extra output (e.g. `whoami`); only keys listed in `[Expect]` are validated

---

## [Expect] (optional validation)

`[Expect]` lists key/value pairs that must appear in the `Check` output.

- Only entries listed in `[Expect]` are validated
- Any other output is ignored (but still logged)
- Typical use case: validate `reg query` output like `AllowTelemetry    REG_DWORD    0x0`

Example:

```ini
[Expect]
AllowTelemetry=0x0
BingSearchEnabled=0x0
```

---

## Real-world examples

### Disable Bing Search (plugin)

```powershell
# Disable Bing Search.ps1

[Commands]
Info=Disables Bing integration in Windows Search
Check=reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v BingSearchEnabled
Do=reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v BingSearchEnabled /t REG_DWORD /d 0 /f && echo BingSearchEnabled set to 0
Undo=reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v BingSearchEnabled /t REG_DWORD /d 1 /f && echo BingSearchEnabled set to 1

[Expect]
BingSearchEnabled=0x0
```


