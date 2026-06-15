# Release process

This document describes how maintainers prepare, build, and publish PC Inform releases. End-user documentation is in the [README](../README.md). Developer context is in [DEVELOPER.md](DEVELOPER.md).

## Overview

A release consists of:

1. Versioned application binaries built from this repository
2. A GitHub Release with attached assets
3. Optional hosted `version.json` for organizations that enable update checks in `appsettings.json`

PC Inform does **not** auto-install updates. When update checking is enabled, the app only notifies the user and opens a download URL in the browser.

## Configuration vs release metadata

Two separate files serve different roles:

### `appsettings.json` (on each end-user PC)

- **Location:** `C:\ProgramData\PCInform\appsettings.json`
- **Audience:** IT administrators / deployment
- **Purpose:** branding, contacts, feature flags, and whether/how to check for updates

Relevant update settings:

```json
{
  "features": {
    "checkUpdates": false
  },
  "update": {
    "enabled": false,
    "versionUrl": ""
  }
}
```

Both `features.checkUpdates` and `update.enabled` must be `true`, and `update.versionUrl` must point to a valid URL, for the application to perform a check on startup.

Public defaults in [appsettings.example.json](../appsettings.example.json) keep update checking **disabled**.

### `version.json` (hosted remotely)

- **Location:** URL chosen by the maintainer or organization (GitHub Release asset, static web server, etc.)
- **Audience:** PC Inform instances configured with `update.versionUrl`
- **Purpose:** advertise latest version, download link, optional release notes

Example: [version.example.json](version.example.json)

| Field | Purpose |
|-------|---------|
| `version` | Latest published version (semver string, compared to running app) |
| `downloadUrl` | Browser download link (e.g. GitHub Release asset URL) |
| `sha256` | Optional integrity hash (informational; app does not verify automatically) |
| `mandatory` | Reserved for future use; not used for forced install |
| `releaseNotesPl`, `releaseNotesEn` | Shown in the update dialog when a newer version exists |

**Relationship:** `appsettings.json` controls *if* and *where* to look; `version.json` describes *what* is available. Organizations host or reference `version.json`; administrators set `update.versionUrl` in deployed `appsettings.json`.

## Preparing a release

1. **Finalize changes** on the release branch or `main` as per your process.
2. **Set version** in `PCInform/PCInform.csproj` (assembly / informational version).
3. **Update** `CHANGELOG.md` with release notes for `X.Y.Z`.
4. **Verify** [appsettings.example.json](../appsettings.example.json) still reflects safe public defaults (TeamViewer, Atera, and update flags disabled).
5. **Build and smoke-test** on Windows (see below).

## Building release binaries

Publish the end-user application and the optional configurator as **separate** outputs before building the installer and GitHub Release assets.

### End-user application (self-contained single file)

On Windows (PowerShell):

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

Output directory:

```
PCInform\bin\Release\net8.0-windows\win-x64\publish\
```

Primary artifact: `PCInform.exe` (used by `PCInform-Setup.exe`)

### Administrator configurator (separate asset)

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

Primary artifact: `PCInform.Configurator.exe` (attach separately to GitHub Releases; **not** included in the standard installer)

### Installer

Requires [Inno Setup 6](https://jrsoftware.org/isinfo.php) on Windows:

```powershell
iscc installer\PCInform.iss
```

Output: `PCInform-Setup.exe` (in the installer output folder configured by Inno Setup).

The end-user installer:

- Installs for all users to `C:\Program Files\PCInform\` (`{autopf}\PCInform`)
- Requires administrator privileges
- Installs `PCInform.exe` and its required publish files only
- Does **not** install `PCInform.Configurator.exe`
- Adds an all-users Start Menu shortcut for **PC Inform**
- Optional all-users desktop shortcut for **PC Inform** only
- Seeds `C:\ProgramData\PCInform\appsettings.json` **only if it does not exist**:
  - from `appsettings.json` next to the setup executable, if present
  - otherwise from `appsettings.example.json`
- Never overwrites existing `C:\ProgramData\PCInform\appsettings.json` during upgrade
- Preserves existing global configuration on upgrade
- Replaces the application executable

### Do not commit binaries

Never commit to git:

- `PCInform.exe`
- `PCInform-Setup.exe`
- `bin/Release/.../publish/` contents

Distribute these via GitHub Releases only.

## Creating a GitHub Release

1. Open the repository on GitHub: `https://github.com/TimeWizard007/pcinform`
2. **Releases** → **Draft a new release**
3. **Choose a tag:** create tag `vX.Y.Z` matching `PCInform.csproj` version (e.g. `v1.0.1`)
4. **Release title:** `PC Inform vX.Y.Z`
5. **Description:** copy relevant section from `CHANGELOG.md`
6. **Attach assets:**
   - **Required for most users:** `PCInform-Setup.exe` (end-user installer; `PCInform.exe` only)
   - **Optional (administrators):** `PCInform.Configurator.exe` (separate portable admin tool; not bundled in the installer)
   - **Optional:** `PCInform.exe` (portable) or a ZIP containing it
   - **Optional:** `version.json` for update-aware deployments
7. Publish the release

### Asset URLs

After upload, GitHub provides stable download URLs, for example:

```
https://github.com/TimeWizard007/pcinform/releases/download/v1.0.1/PCInform-Setup.exe
```

Use this pattern in `version.json` → `downloadUrl`.

## Maintaining `version.json`

When you publish a release and want update-aware deployments to find it:

1. Copy [version.example.json](version.example.json) as a starting point.
2. Set `version` to the new release (e.g. `1.0.1`).
3. Set `downloadUrl` to the GitHub Release asset URL for the installer.
4. Optionally set `sha256` (hash of the installer file) and localized `releaseNotesPl` / `releaseNotesEn`.
5. Host the file:
   - **Option A:** Attach `version.json` to the GitHub Release and use that asset URL as `versionUrl`
   - **Option B:** Publish to a static URL your organization controls (CDN, internal web server)

Example for release `v1.0.1`:

```json
{
  "version": "1.0.1",
  "downloadUrl": "https://github.com/TimeWizard007/pcinform/releases/download/v1.0.1/PCInform-Setup.exe",
  "sha256": "",
  "mandatory": false,
  "releaseNotesPl": "Poprawki i usprawnienia.",
  "releaseNotesEn": "Fixes and improvements."
}
```

Administrators enable checking in deployed `appsettings.json`:

```json
{
  "features": {
    "checkUpdates": true
  },
  "update": {
    "enabled": true,
    "versionUrl": "https://github.com/TimeWizard007/pcinform/releases/download/v1.0.1/version.json"
  }
}
```

Update `versionUrl` when you move to a canonical “latest” URL, or repoint deployments when you publish each release.

## Post-release checklist

- [ ] Git tag `vX.Y.Z` matches assembly version
- [ ] `CHANGELOG.md` updated
- [ ] `PCInform-Setup.exe` attached to GitHub Release
- [ ] Release notes visible on GitHub
- [ ] `version.json` updated/hosted if update checks are used
- [ ] No binaries committed to the source repository

## What PC Inform does not do

- No automatic download or install of updates
- No background updater or launcher
- No SMTP or server-side email
- Update failure never blocks application startup
