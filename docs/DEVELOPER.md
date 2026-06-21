# Developer guide

This document is for contributors and maintainers. End-user and organization documentation is in the [README](../README.md).

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11 for local WinForms debugging, **or** Linux/macOS with .NET SDK for cross-publish to `win-x64`
- [Inno Setup 6](https://jrsoftware.org/isinfo.php) (Windows) to build the installer

## Project structure

```
pcinform/
├── PCInform/                    # End-user WinForms application (.NET 8)
├── PCInform.Configurator/       # Administrator config editor (PCInform.Configurator.exe)
├── PCInform.Shared/             # Shared configuration models and persistence
│   ├── Models/                  # AppSettings JSON model
│   └── Configuration/           # AppPaths, ConfigurationService, validation
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
- **Scope:** branding, support contacts, UI visibility (`features`), report content (`report`), optional update source

The application does **not** read config from `%LOCALAPPDATA%` or from beside the executable.

Implementation entry points:

- `PCInform.Shared/Configuration/AppPaths.cs` — path constants
- `PCInform.Shared/Configuration/ConfigurationService.cs` — load, save, merge defaults, legacy `support.email` migration, `report` section upgrade
- `PCInform.Shared/Configuration/ConfigurationValidator.cs` — validation for the configurator
- `PCInform.Shared/Configuration/VisibilityHelper.cs` — contact/section visibility rules
- `PCInform.Shared/Models/AppSettings.cs` — JSON model
- `PCInform.Configurator/` — administrator WinForms editor for `appsettings.json` (Application, Support, Features, Report, Update tabs)

### Config schema upgrade

On startup and when loading config in the configurator, `ConfigurationService`:

- Merges missing top-level sections with safe defaults (`application`, `support`, `features`, `report`, `update`)
- Adds missing v1.2 fields when absent from older JSON (`features.showNetworkStatus`, `report.includeNetworkStatus`, `update.showFooterIndicator`)
- Adds a missing `report` section by deriving values from existing `features` flags (preserves prior report behaviour for older configs)
- Saves upgraded global config back to `C:\ProgramData\PCInform\appsettings.json` only when new fields were added
- Preserves all existing field values and does not remove unknown JSON properties on deserialize
- Does not overwrite an existing `appsettings.json` on disk unless the app or configurator explicitly saves

UI visibility is controlled by `features.show*`. Clipboard and **Report problem** content uses `report.include*` independently.

Diagnostic logs (when writable): `C:\ProgramData\PCInform\Logs\PCInform.log`

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
dotnet run --project PCInform.Configurator\PCInform.Configurator.csproj
```

### Release publish (single-file, self-contained)

Publish the end-user application:

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

Publish the optional administrator configurator separately (not bundled in `PCInform-Setup.exe`):

```powershell
dotnet publish PCInform.Configurator\PCInform.Configurator.csproj `
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
PCInform.Configurator\bin\Release\net8.0-windows\win-x64\publish\PCInform.Configurator.exe
```

Cross-publish from Linux:

```bash
dotnet publish PCInform/PCInform.csproj -c Release -r win-x64 --self-contained true \
  /p:PublishSingleFile=true /p:PublishTrimmed=false \
  /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true

dotnet publish PCInform.Configurator/PCInform.Configurator.csproj -c Release -r win-x64 --self-contained true \
  /p:PublishSingleFile=true /p:PublishTrimmed=false \
  /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true
```

### Installer

After publish, compile the Inno Setup script on Windows:

```powershell
iscc installer\PCInform.iss
```

The end-user installer script installs `PCInform.exe` only (not the configurator) for all users to `C:\Program Files\PCInform\`, and creates `C:\ProgramData\PCInform\appsettings.json` with `onlyifdoesntexist` (from `appsettings.json` next to the installer when present, otherwise from `appsettings.example.json`).

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
| `PCInform-Setup.exe` | Primary end-user installer (`PCInform.exe` only) |
| `PCInform.Configurator.exe` | Optional administrator config editor (separate GitHub Release asset) |
| `PCInform.exe` (optional) | Portable binary or ZIP |
| `version.json` (optional) | Remote metadata for `update.versionUrl` |

Example `version.json`: [version.example.json](version.example.json)

Relationship to `appsettings.json`:

- **`appsettings.json`** on each PC decides *whether* to check for updates (`update.enabled`, `update.versionUrl`, `update.showFooterIndicator`) and branding/features/report visibility
- **`version.json`** on a server or GitHub Release describes *what* the latest published version is and where to download it

PC Inform compares remote `version.json` to the running assembly version using semantic version rules. When a newer version is available, it shows a discreet footer indicator (and About notice). Clicking the indicator opens `downloadUrl` in the browser. It does **not** auto-install, show a startup popup, or run an updater.

## Key components (quick reference)

| Area | Files |
|------|--------|
| Startup | `Program.cs` |
| Main UI | `MainForm.cs` |
| Reports / clipboard | `Services/ReportFormatter.cs` |
| Email drafts | `Services/MailHelper.cs`, `OutlookMailService.cs` |
| Update check | `Services/UpdateService.cs`, `Services/VersionHelper.cs` (informational footer indicator) |
| Network status | `Services/NetworkStatusService.cs` (footer indicator) |
| Diagnostics | `PCInform.Shared/Configuration/AppDiagnosticLog.cs` |
| System info | `Services/SystemInfoService.cs` |

## License

MIT — see [LICENSE](../LICENSE).
