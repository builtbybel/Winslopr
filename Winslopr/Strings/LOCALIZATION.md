# Localization Guide

Winslopr uses the standard WinUI 3 resource system (`.resw` files) for translations.
This guide explains how translations work and how you can contribute.

---

## For Translators

You don't need Visual Studio, C#, or any programming knowledge.
All you need is a text editor and a GitHub account.

### How translation files work

Every language has its own folder under `Strings/` with a `Resources.resw` file:

```
Strings/
  en-US/Resources.resw    <-- English (default)
  de-DE/Resources.resw    <-- German
```

The `.resw` file is simple XML. Each line has a **key** (don't touch) and a
**value** (translate this):

```xml
<data name="NavHome.Text"><value>Home</value></data>
```

The file is organized in clearly labeled sections so you always know
what part of the app you're translating:

```xml
<!-- XAML x:Uid: MainWindow — Navigation -->
<!-- XAML x:Uid: InstallPage -->
<!-- C# ResourceLoader: Donation dialog -->
```

### How to create a translation

1. **Copy** `Strings/en-US/Resources.resw` into a new folder, e.g. `Strings/fr-FR/`
2. **Translate** every `<value>...</value>` — do NOT change the `name` attributes
3. **Keep placeholders** like `{0}`, `{1}` — they get replaced with numbers/text at runtime
4. **Keep special characters** like `&lt;` (this is `<` in XML) and `&#x2764;` (heart emoji)
5. **Don't translate** brand names: Winslopr, Copilot, Edge, Ko-fi, PayPal, CFEnhancer

**Example:**

```xml
<!-- English original -->
<data name="BtnAnalyzeText.Text"><value>Inspect system</value></data>
<data name="Tools_Loaded"><value>{0} extensions loaded.</value></data>

<!-- French translation -->
<data name="BtnAnalyzeText.Text"><value>Inspecter le système</value></data>
<data name="Tools_Loaded"><value>{0} extensions chargées.</value></data>
```

### How to submit your translation

1. Fork the repo: https://github.com/builtbybel/Winslop
2. Add your `Strings/<locale>/Resources.resw` file
3. Open a Pull Request with the title: **Add \<Language\> translation**

That's it! The developer will handle the project configuration, menu entry,
and compilation. You just provide the translated `.resw` file.

**Tips for a good translation:**
- Keep the same tone — Winslopr is informal and direct
- The section comments in the file tell you which page each group belongs to
- If a value looks technical or unclear, check the English version in context
  by looking at the section header (e.g. "ToolsPage", "Donation dialog")

---

## For Developers

Technical details for integrating a new language into the build.

### Adding a new language (2 steps)

**1. Create the resource file**

Copy `Strings/en-US/Resources.resw` to `Strings/<locale>/Resources.resw`
(e.g. `Strings/fr-FR/Resources.resw`) and translate all values.

**2. Register the language in the build**

Add the locale to `SatelliteResourceLanguages` in `Winslop.csproj`:

```xml
<SatelliteResourceLanguages>en;de;fr</SatelliteResourceLanguages>
```

That's it. No XAML changes, no code changes needed.

### How it works

The language switcher menu is **fully dynamic**. At startup,
`MainWindow.BuildLanguageMenu()` scans the `Strings/` subfolders
next to the exe, finds every folder with a `Resources.resw` file,
and creates a menu item for each language automatically. The language
name is displayed in its native script via `CultureInfo.NativeName`
(e.g. "Deutsch", "Français"). A checkmark icon marks the active language.

The `Strings/` folders are copied to the output directory automatically
by the `.csproj` config:

```xml
<Content Include="Strings\**\Resources.resw">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```

This means the `Strings/` folder **must be shipped** with the release.
It is used at runtime for language detection. The compiled resources
(`Winslop.pri`) contain the actual translations; the `Strings/` folders
serve as a manifest of available languages.

### Build and test

Build the project. `MakePri` compiles all `.resw` files into
`Winslop.pri`. Switch language via **Settings > Language** and
restart the app.

### Key types reference

| Pattern | Used by | Example |
|---|---|---|
| `Name.Property` | XAML `x:Uid` (auto-applied at load) | `NavHome.Text`, `SearchBox.PlaceholderText` |
| `Name` | C# via `Localizer.Get()` / `GetFormat()` | `Analysis_Complete`, `Tools_Loaded` |

### Architecture

| File | Purpose |
|---|---|
| `Strings/<locale>/Resources.resw` | Translated strings per language |
| `Helpers/Localizer.cs` | `Get()`, `GetFormat()`, `SwitchLanguageAsync()`, `CurrentLanguage` |
| `Helpers/SettingsHelper.cs` | Persists language choice to `Winslop.txt` |
| `App.xaml.cs` | Applies `PrimaryLanguageOverride` on startup |
| `MainWindow.xaml.cs` | `BuildLanguageMenu()` — dynamic language discovery |
| `Winslop.csproj` | `SatelliteResourceLanguages` + copies `Strings/` to output |

---

## Questions?

Open an issue at https://github.com/builtbybel/Winslop/issues
