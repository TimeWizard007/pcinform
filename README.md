# PC Inform

**PC Inform** (`pcinform`) is a lightweight, open-source Windows desktop utility for help desks and IT teams. It collects essential PC and user information and makes it easy to copy or email support details — without requiring administrator rights.

## Features

- **Computer information:** name, AD domain, OS, IPv4, DNS, uptime, manufacturer/model, BIOS serial, device type
- **User information:** login (`DOMAIN\username`) and display name
- **Optional agents:** TeamViewer status/launch, optional Atera detection (disabled in public defaults)
- **Support workflow:** copy formatted report to clipboard, open prefilled support email
- **Email integration:** Outlook Classic COM draft (when a MAPI profile exists) with `mailto:` fallback
- **Localization:** Polish and English UI
- **Configuration:** fully customizable via `appsettings.json`
- **Updates:** optional remote version check (configured by administrators)
- **No admin required:** runs with standard user permissions

## Configuration

`appsettings.json` controls branding, support contact details, feature toggles, and update behavior.

On first run, PC Inform creates a default configuration at:

```
%LOCALAPPDATA%\PCInform\appsettings.json
```

You can also place `appsettings.json` next to the executable for portable deployments.

See [appsettings.example.json](appsettings.example.json) for all available options:

| Section | Purpose |
|---------|---------|
| `application` | App name, window title, banner text, default language, accent color |
| `support` | Company name, email, phone, email subject prefixes |
| `features` | TeamViewer, Atera, update check toggles |
| `update` | Remote `version.json` URL and enable flag |

User language preference is stored separately at:

```
%APPDATA%\PCInform\settings.json
```

### Administrator vs end user

- **Administrators / deployment:** customize `appsettings.json` for your organization (support email, banner, accent color, optional update URL).
- **Update settings** (`features.checkUpdates`, `update.enabled`, `update.versionUrl`) are intended for **administrator or deployment configuration**, not casual end-user changes.
- **End users** normally edit only language (in-app) and use the app — they should not need to change update configuration.

Public defaults disable Atera detection and automatic update checks. Enable them in your deployed `appsettings.json` when needed.

## Installation

### End users (installer)

1. Download `PCInform-Setup.exe` from [GitHub Releases](https://github.com/TimeWizard007/pcinform/releases).
2. Run the installer — **no administrator rights required**.
3. The app installs to `%LOCALAPPDATA%\PCInform\`.
4. Optionally create a desktop shortcut during setup.

Existing `appsettings.json` files are preserved during upgrades.

### Portable

Copy `PCInform.exe` (and optionally `appsettings.json`) to any folder and run directly.

## Update mechanism

When enabled in **administrator-provided** `appsettings.json`, PC Inform checks a remote `version.json` on startup.

Example format — see [docs/version.example.json](docs/version.example.json).

Behavior:

- Compares remote version with the running assembly version
- Shows a localized Yes/No dialog if a newer version exists
- Opens `downloadUrl` in the default browser when the user chooses Yes
- Does **not** silently install updates
- Failures do **not** block application startup

See [docs/RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md) for the full release and `version.json` workflow.

## Build from source

### Requirements

- Windows 10 or 11 (or Linux with .NET SDK for cross-publish)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Debug run

```powershell
dotnet run --project PCInform\PCInform.csproj
```

### Release publish

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

Replace `PCInform\icon.ico` with your own icon before publishing.

## Release process

See [docs/RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md).

Summary:

1. Update version in `PCInform/PCInform.csproj` and `CHANGELOG.md`.
2. Publish and build `PCInform-Setup.exe` with Inno Setup.
3. Create a GitHub Release and attach the installer (do **not** commit binaries to the source tree).
4. Publish `version.json` for deployments that enable update checks.

## Project structure

```
pcinform/
├── PCInform/              # Application source
├── appsettings.example.json
├── docs/
│   ├── version.example.json
│   └── RELEASE_PROCESS.md
├── installer/PCInform.iss
├── LICENSE
└── CHANGELOG.md
```

## License

This project is licensed under the [MIT License](LICENSE).

## Security

- No secrets, tokens, or private URLs are embedded in the application.
- Update downloads are opened in the browser only — installers are never auto-run.
- Configuration is local to the user machine.
