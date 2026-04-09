# Winslop — Quick Start

Winslop is a WinUI 3 desktop app for Windows 10/11 that removes bloat, ads, telemetry and other "slop" from your system.

Quick links:
- [Feature Reference](features.md)
- [Plugins Reference](plugins.md)
- [Extensions Reference](extensions.md)
- [Windows 11 Annoyances](https://builtbybel.github.io/Winslopr/annoyances/) — 59 community-sourced annoyances with fixes

---

## Getting started

1. Run **Winslopr.exe**
2. Click **Inspect system** — scans your system and highlights issues
3. Review the results in the feature tree (checkboxes)
4. Click **Apply selected changes** to fix checked items

That's it. No account, no setup wizard, no cloud.

---

## Navigation

Winslopr uses a left sidebar with five sections:

| Icon | Tab | What it does |
|------|-----|-------------|
| 🏠 | **Home** | Feature tree — system tweaks grouped by category (Ads, Privacy, Edge, Taskbar, AI, etc.) |
| 📦 | **Apps** | Scan installed apps and remove selected ones |
| 📥 | **Install** | Install apps via winget in bulk |
| 🔧 | **Tools** | PowerShell extensions — extra utilities that ship with the app |
| ⚙️ | **Settings** | Language, theme, backdrop, compatibility check, about |

Bottom of the sidebar:
- **💗 Support** — Ko-fi / PayPal links
- **⚙️ Settings** — app configuration

---

## Home tab (Feature tree)

The main view. Features are grouped into categories:

- **Ads** — Start Menu ads, lock screen tips, settings promotions, etc.
- **Privacy** — Telemetry, activity history, location tracking, diagnostic data
- **AI** — Copilot, Recall, Click to Do, Bing search, search suggestions
- **Edge** — Browser hijacking, sidebar, shopping assistant, import on launch
- **Taskbar** — Widgets, Chat icon, search box, Task View button
- **System** — Shutdown speed, BSOD details, hibernation, network throttling
- **UI** — Context menus, Snap Assist, Start layout, dark mode
- **Gaming** — Game DVR, power throttling, visual effects

### How to use

1. **Inspect system** — analyzes all features and shows their current state
2. Check/uncheck items you want to fix
3. **Apply selected changes** — applies only the checked items

### Profiles

Use the profile dropdown above the tree to load preset selections:
- Select a profile → Inspect → Apply
- Export your own selection as a `.sel` file
- Import a `.sel` file to restore it later

### Context menu (right-click)

Right-click any item in the tree:
- **Analyze** — check this specific item
- **Fix** — apply this item only
- **Restore** — revert to Windows default
- **Help** — show what it does

---

## Apps tab

Scans installed Windows apps (UWP/Store apps) and lets you remove selected ones.

1. Click **Inspect system** to scan
2. Check apps you want to remove
3. Click **Apply selected changes**

---

## Install tab

Bulk-install apps via winget.

1. Browse or search available apps
2. Check what you want
3. Click **Apply selected changes** to install

_App list adapted from [WinUtil by Chris Titus Tech](https://github.com/ChrisTitusTech/winutil)._

---

## Tools tab

PowerShell-based extensions that ship with Winslopr. Select a tool on the left, configure options on the right, click **Run**.

Included tools:
- **Post-setup Cleanup** — disk cleanup, clear update cache, remove Windows.old
- **Explorer Tweaks** — show file extensions, hidden files, change default view
- **Power Actions** — restart Explorer, shutdown, restart, sleep
- **Windows Update Tamer** — control Windows Update behavior
- **RemoveWindowsAI** — force-remove Copilot, Recall, AI Actions
- **Microsoft Defender Maintenance** — update signatures, repair, show status
- **Winget Bulk Installer** — install multiple apps at once
- **Create Restore Point** — one-click system restore point
- **And more...**

Tools show a yellow **help hint** when documentation is available.
Click **Uninstall** to remove any extension (asks for confirmation).

---

## Settings

- **Language** — switch between English, German, French, Russian, Chinese (requires restart)
- **Theme** — System / Light / Dark (requires restart)
- **Backdrop** — Mica / Mica Alt (live switch)
- **Compatibility** — shows if your Windows version is fully compatible
- **Extensions** — manage plugins, open Tools page
- **About** — version, GitHub link, update check, feedback

---

## Top bar

- **Search** — filters the current view (features, apps, or tools)
- **[···] Menu** — additional actions (log inspector, refresh, etc.)

---

## Logger

The bottom panel shows a live log of all actions. Use the **[···] menu → Log Inspector** to view logs in a clean, structured format online.

---

## Safety

- Winslopr shows exactly what it will change before applying
- Nothing runs automatically — you choose what to fix
- Changes can be reverted via right-click → Restore
- No data is uploaded — everything stays local
- Create a restore point before making changes (available in Tools)
