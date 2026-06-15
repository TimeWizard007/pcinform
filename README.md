# PC Inform

**PC Inform** is a lightweight Windows desktop utility for help desks and IT teams. It collects essential PC and user information and makes it easy to copy details or prepare a support email — without requiring administrator rights to run the application.

Polish and English UI are supported. Branding, contact details, visible fields, and optional behaviors are controlled by a single machine-wide configuration file.

## Features

- **Computer information:** name, AD domain, OS, IPv4, DNS, uptime, manufacturer/model, BIOS serial, device type (each field can be shown or hidden)
- **User information:** login and display name (optional)
- **Contact section:** configurable email, phone, mobile phone, and website
- **Support workflow:** copy a formatted report to the clipboard, open a prefilled **Report problem** email draft
- **Email integration:** Outlook Classic draft when a MAPI profile exists, with `mailto:` fallback; CC/BCC used when configured
- **Localization:** Polish and English; language switcher can be limited to one language by configuration
- **About dialog:** version, description, author, and project link
- **Optional update notice:** administrators can enable a remote version check (disabled by default; opens a download page in the browser only — no automatic install)

## Requirements

- Windows 10 or 11 (64-bit)
- Standard user account to **run** PC Inform (no admin required at runtime)
- Administrator rights to **install** or upgrade via the setup package
- A mail client for **Report problem** (for example Outlook or any application registered for `mailto:`)

## Installation

### Installer (recommended)

1. Download **PCInform-Setup.exe** from [GitHub Releases](https://github.com/TimeWizard007/pcinform/releases).
2. Run the installer **as administrator**.
3. PC Inform is installed for all users under `C:\Program Files\PCInform\`.
4. On first install, the installer creates `C:\ProgramData\PCInform\appsettings.json` if that file does not already exist (from `appsettings.example.json`, or from `appsettings.json` placed next to the installer).
5. Existing global configuration is **never overwritten** during upgrade.
6. Optionally create an all-users desktop shortcut during setup (Start Menu shortcuts are always created).

### Portable use

You can run **PCInform.exe** directly. Configuration still comes from the global file below (create it manually or let PC Inform create safe defaults on first run).

## Configuration

PC Inform is configured through **one machine-wide** `appsettings.json` file shared by all users on the computer.

### Where configuration is stored

```
C:\ProgramData\PCInform\appsettings.json
```

Environment variable form: `%PROGRAMDATA%\PCInform\appsettings.json`

PC Inform does **not** use `%LOCALAPPDATA%\PCInform\appsettings.json` or a file next to the executable.

On first run, if the global file is missing, PC Inform creates the folder and a default file with safe public settings. An existing file is never modified automatically.

Per-user **language preference** only is stored at `%APPDATA%\PCInform\settings.json`.

See [appsettings.example.json](appsettings.example.json) for a full example.

### PC Inform Configurator (optional admin tool)

**PCInform.Configurator.exe** is a separate administrator tool for creating and editing `C:\ProgramData\PCInform\appsettings.json`. It is **not** included in the standard end-user installer and is **not** part of the end-user application.

Download it separately from [GitHub Releases](https://github.com/TimeWizard007/pcinform/releases) when you need a graphical editor for Application, Support, Features, Report, and Update settings. The configurator validates settings before saving (for example at least one language enabled, and `versionUrl` when updates are enabled).

End users normally do not need the configurator — IT administrators prepare the machine-wide configuration before or after deployment, or edit `appsettings.json` directly.

### JSON sections overview

| Section | Purpose |
|---------|---------|
| **Application** | App name, window title, banner text, default language, accent color, enabled languages |
| **Support** | Company name, support email/phone/mobile/website, CC/BCC, subject prefixes, contact visibility flags |
| **Features** | Which computer/user fields are shown in the main window, optional TeamViewer/Atera integration, update-check permission |
| **Report** | Which fields are included in copied clipboard reports and **Report problem** email content |
| **Update** | Whether to check a remote `version.json` and which URL to use (disabled by default) |

**Display vs report:** `features.show*` flags control **main window visibility only**. The separate `report.include*` flags control **clipboard reports** and **Report problem** email content. You can show a minimal UI while still including full diagnostics in reports, or the reverse. These flags do **not** stop the application from collecting system information internally.

### Application (`application`)

| Setting | Purpose |
|---------|---------|
| `name`, `windowTitle`, `bannerText` | Application name and banner text |
| `defaultLanguage` | Default UI language (`pl` or `en`) when the user has no saved preference |
| `accentColor` | Accent color (hex, e.g. `#E87722`) |
| `enablePolish`, `enableEnglish` | Which languages are available; if only one is enabled, the language switcher is hidden |

### Support (`support`)

| Setting | Purpose |
|---------|---------|
| `companyName` | Organization name (used in the contact section title when enabled) |
| `emailTo` | Support recipient for **Report problem** and displayed contact email |
| `emailCc`, `emailBcc` | Optional CC/BCC on support email drafts |
| `emailSubjectPrefixPl`, `emailSubjectPrefixEn` | Subject prefix by language |
| `phone` | Landline / main phone (e.g. `+48 22 123 45 67`) |
| `mobilePhone` | Mobile phone (e.g. `+48 500 600 700`) |
| `showCompanyName`, `showEmail`, `showPhone`, `showMobilePhone`, `showWebsite` | Visibility of each contact item |

**Support email behavior:**

- **Report problem** creates a draft in the user's mail client only (no SMTP sending).
- The **From** address is always the user's mail account — it is not configured in PC Inform.
- Disabled contact fields are hidden from the UI, clipboard report, and email body.

Phone labels in the UI: Polish *Infolinia* / *Telefon komórkowy*; English *Phone* / *Mobile phone*.

### Website configuration

Website URL is set under `support.websiteUrl`. Display is controlled by `support.showWebsite`.

- When enabled and a URL is set, the contact section shows a clickable link (Polish: **Strona WWW**, English: **Website**).
- When the URL is empty or `showWebsite` is `false`, the website row is hidden.

`application.websiteUrl` is available for future or custom use; the contact section uses `support.websiteUrl`.

### Features (`features`)

Organizations can show a **minimal** UI (banner + contact + Report problem) or a **full diagnostic** view by editing `appsettings.json` — no recompile required.

**UI field visibility** (default **true** unless noted):

- Computer: `showComputerName`, `showDomain`, `showOperatingSystem`, `showIpAddress`, `showDnsServers`, `showUptime`, `showManufacturerModel`, `showSerialNumber`, `showDeviceType`
- User: `showUserLogin`, `showDisplayName`
- TeamViewer section: `showTeamViewerSection` (default **false**)

Disabled `show*` fields are omitted from the **main window** only.

### Report (`report`)

Controls which fields appear in **copied clipboard reports** and **Report problem** email drafts (default **true** unless noted):

- Computer/user: `includeComputerName`, `includeDomain`, `includeOperatingSystem`, `includeIpAddress`, `includeDnsServers`, `includeUptime`, `includeManufacturerModel`, `includeSerialNumber`, `includeDeviceType`, `includeUserLogin`, `includeDisplayName`
- Agents: `includeTeamViewer`, `includeAtera` (default **false**)

When upgrading from older configs without a `report` section, PC Inform derives initial report flags from the previous `features` visibility settings so existing deployments keep similar report content.

**Optional integrations** (default **false** in public configuration):

| Flag | Purpose |
|------|---------|
| `showTeamViewer` | Detect TeamViewer for reports |
| `allowLaunchTeamViewer` | Show launch button when TeamViewer is installed |
| `detectAtera` | Detect Atera agent |
| `showAteraInGui` | Show Atera in the UI |
| `includeAteraInReports` | Legacy report flag (superseded by `report.includeAtera`; kept for compatibility) |
| `checkUpdates` | Allow update check on startup |

### Update (`update`)

Update checking is **disabled by default**. PC Inform does not install updates automatically.

| Setting | Purpose |
|---------|---------|
| `enabled` | Master switch for update check (default `false`) |
| `versionUrl` | URL of remote `version.json` (default empty) |

Both `update.enabled` and `features.checkUpdates` must be enabled for a check to run. When a newer version is found, the user sees a Yes/No dialog; choosing Yes opens the download URL in the default browser.

Example remote metadata format: [docs/version.example.json](docs/version.example.json).

Administrators who enable updates must host `version.json` and point `versionUrl` to it. End users normally should not change update settings.

## GitHub Releases

Official builds are published at:

**https://github.com/TimeWizard007/pcinform/releases**

Typical assets:

- **PCInform-Setup.exe** — recommended end-user installer (does not include the configurator)
- **PCInform.Configurator.exe** — optional administrator config editor (download separately when needed)
- **PCInform.exe** — optional portable binary (when provided)
- **version.json** — optional metadata for deployments that enable update checks

Download the installer from the latest release unless your IT team provides an internal package.

## Security notes

- No secrets or private URLs are embedded in the application.
- Update downloads open in the browser only; installers are never run automatically.
- Company, support, and feature configuration is machine-wide; only UI language preference is per user.

## License

This project is licensed under the [MIT License](LICENSE).

## For developers

Build instructions, project layout, and release workflow are documented in [docs/DEVELOPER.md](docs/DEVELOPER.md) and [docs/RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md).
