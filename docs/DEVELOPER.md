# Developer guide

This document is for contributors and maintainers. End-user and organization documentation is in the [README](../README.md).

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 for local WinForms debugging, **or** Linux/macOS with .NET SDK for cross-publish to `win-x64`
- [Inno Setup 6](https://jrsoftware.org/isinfo.php) (Windows) to build the installer

## Project structure

```
pcinform/
├── PCInform/                    # WinForms application (.NET 8)
│   ├── Configuration/           # AppPaths, ConfigurationService, VisibilityHelper
│   ├── Localization/            # PL/EN strings and language resolution
│   ├── Models/                  # AppSettings and data models
│   ├── Services/                # System info, mail, reports, updates, settings
│   ├── UI/                      # Theme and custom controls
│   ├── MainForm.cs
│   ├── AboutForm.cs
│   └── Program.cs
├── appsettings.example.json     # Reference configuration (safe public defaults)
├── docs/
│   ├── DEVELOPER.md             # This file
│   ├── RELEASE_PROCESS.md       # Release and version.json workflow
│   └── version.example.json     # Remote update metadata example
├── installer/
│   └── PCInform.iss             # Inno Setup script
├── LICENSE
└── CHANGELOG.md
```

Solution file: `pcinform.sln`

## Configuration architecture

PC Inform uses **two configuration layers**:

### 1. Machine-wide `appsettings.json` (deployment)

- **Path:** `C:\ProgramData\PCInform\appsettings.json` (`%PROGRAMDATA%\PCInform\`)
- **Loaded by:** `ConfigurationService` at startup
- **Created by:** installer (if missing) or application first run (if missing and writable)
- **Never overwritten** on upgrade or by the app when the file already exists
- **Scope:** branding, support contacts, feature visibility, optional update source

The application does **not** read config from `%LOCALAPPDATA%` or from beside the executable.

Implementation entry points:

- `PCInform/Configuration/AppPaths.cs` — path constants
- `PCInform/Configuration/ConfigurationService.cs` — load, merge defaults, legacy `support.email` migration
- `PCInform/Configuration/VisibilityHelper.cs` — contact/section visibility rules
- `PCInform/Models/AppModels.cs` — JSON model

### 2. Per-user `settings.json` (language only)

- **Path:** `%APPDATA%\PCInform\settings.json`
- **Content:** `{ "language": "pl" | "en" }`
- **Managed by:** `SettingsService` / in-app language switcher
- Constrained by `application.enablePolish` / `application.enableEnglish` in global config

### Safe public defaults

`ConfigurationService.CreateDefaultSettings()` and `appsettings.example.json` use conservative defaults:

- TeamViewer and Atera integration flags: **false**
- `features.checkUpdates`: **false**
- `update.enabled`: **false**
- `update.versionUrl`: `""`

## Build instructions

### Debug run (Windows)

```powershell
dotnet run --project PCInform\PCInform.csproj
```

### Release publish (single-file, self-contained)

```powershell
dotnet publish PCInform\PCInform.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:PublishTrimmed=false `
  /p:IncludeNativeLibrariesForSelfExtract=true `
  /p:EnableCompressionInSingleFile=true
```

Output:

```
PCInform\bin\Release\net8.0-windows\win-x64\publish\PCInform.exe
```

Cross-publish from Linux:

```bash
dotnet publish PCInform/PCInform.csproj -c Release -r win-x64 --self-contained true \
  /p:PublishSingleFile=true /p:PublishTrimmed=false \
  /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true
```

### Installer

After publish, compile the Inno Setup script on Windows:

```powershell
iscc installer\PCInform.iss
```

The script installs the app to `%LOCALAPPDATA%\PCInform\` and creates `%PROGRAMDATA%\PCInform\appsettings.json` with `onlyifdoesntexist`.

Replace `PCInform/icon.ico` before release if the icon changes.

## Versioning

- **Assembly version:** `PCInform/PCInform.csproj` (`Version`, `AssemblyVersion`, or `InformationalVersion` as defined in the project)
- **Runtime display:** `AppInfoService` reads the assembly informational version for the About dialog and update comparison
- **Changelog:** update `CHANGELOG.md` for each release
- **Git tag:** use `vX.Y.Z` (e.g. `v1.0.1`) matching the release

Keep the published assembly version, GitHub release tag, and `version.json` `version` field aligned.

## Release workflow (summary)

Full steps: [RELEASE_PROCESS.md](RELEASE_PROCESS.md)

1. Bump version in `PCInform.csproj` and `CHANGELOG.md`
2. `dotnet publish` (Release, `win-x64`)
3. `iscc installer\PCInform.iss` → `PCInform-Setup.exe`
4. Create GitHub Release `vX.Y.Z`
5. Attach release assets (see below)
6. Update hosted `version.json` if your deployment enables update checks

**Do not commit** build outputs (`publish/`, `PCInform.exe`, `PCInform-Setup.exe`) to the repository.

## GitHub release assets

| Asset | Purpose |
|-------|---------|
| `PCInform-Setup.exe` | Primary end-user installer |
| `PCInform.exe` (optional) | Portable binary or ZIP |
| `version.json` (optional) | Remote metadata for `update.versionUrl` |

Example `version.json`: [version.example.json](version.example.json)

Relationship to `appsettings.json`:

- **`appsettings.json`** on each PC decides *whether* to check for updates (`features.checkUpdates`, `update.enabled`) and *where* (`update.versionUrl`)
- **`version.json`** on a server or GitHub Release describes *what* the latest published version is and where to download it

PC Inform compares remote `version.json` to the running assembly version and opens `downloadUrl` in the browser on user confirmation. It does **not** auto-install or run an updater.

## Key components (quick reference)

| Area | Files |
|------|--------|
| Startup | `Program.cs` |
| Main UI | `MainForm.cs` |
| Reports / clipboard | `Services/ReportFormatter.cs` |
| Email drafts | `Services/MailHelper.cs`, `OutlookMailService.cs` |
| Update check | `Services/UpdateService.cs` (no-op when disabled) |
| System info | `Services/SystemInfoService.cs` |

## License

MIT — see [LICENSE](../LICENSE).
