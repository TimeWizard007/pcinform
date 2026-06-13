# PC Inform

**PC Inform** (`pcinform`) is a lightweight, open-source Windows desktop utility for help desks and IT teams. It collects essential PC and user information and makes it easy to copy or email support details — without requiring administrator rights.

## Features

- **Computer information:** name, AD domain, OS, IPv4, DNS, uptime, manufacturer/model, BIOS serial, device type
- **User information:** login (`DOMAIN\username`) and display name
- **Optional agents:** TeamViewer status/launch, Atera detection (reports only by default)
- **Support workflow:** copy formatted report to clipboard, open prefilled support email
- **Email integration:** Outlook Classic COM draft (when a MAPI profile exists) with `mailto:` fallback
- **Localization:** Polish and English UI
- **Configuration:** fully customizable via `appsettings.json`
- **Updates:** optional remote version check (user confirms download in browser)
- **No admin required:** runs with standard user permissions

## Configuration

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
| `update` | Remote `version.json` URL |

User language preference is stored separately at:

```
%APPDATA%\PCInform\settings.json
```

## Installation

### End users (installer)

1. Download `PCInform-Setup.exe` from your release page.
2. Run the installer — **no administrator rights required**.
3. The app installs to `%LOCALAPPDATA%\PCInform\`.
4. Optionally create a desktop shortcut during setup.
5. Edit `%LOCALAPPDATA%\PCInform\appsettings.json` to match your organization.

Existing `appsettings.json` files are preserved during upgrades.

### Portable

Copy `PCInform.exe` (and optionally `appsettings.json`) to any folder and run directly.

## Update mechanism

When enabled in configuration, PC Inform checks a remote `version.json` on startup.

Example format — see [docs/version.example.json](docs/version.example.json):

```json
{
  "version": "1.0.1",
  "downloadUrl": "https://example.com/pcinform/latest/PCInform-Setup.exe",
  "sha256": "PUT_SHA256_HASH_HERE",
  "mandatory": false,
  "releaseNotesPl": "Poprawki i usprawnienia.",
  "releaseNotesEn": "Fixes and improvements."
}
```

Behavior:

- Compares remote version with the running assembly version
- Shows a localized Yes/No dialog if a newer version exists
- Opens `downloadUrl` in the default browser when the user chooses Yes
- Does **not** silently install updates
- Failures do **not** block application startup

## Build from source

### Requirements

- Windows 10 or 11
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

1. Update `Version` in `PCInform/PCInform.csproj`.
2. Update `CHANGELOG.md`.
3. Publish the Release build (command above).
4. Build the installer with [Inno Setup 6](https://jrsoftware.org/isinfo.php):

   ```powershell
   iscc installer\PCInform.iss
   ```

5. Upload `PCInform-Setup.exe` to your release host.
6. Publish an updated `version.json` pointing to the new installer.

## Project structure

```
pcinform/
├── PCInform/              # Application source
├── appsettings.example.json
├── docs/version.example.json
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
