# Release process

This document describes how to publish PC Inform releases for administrators and maintainers.

## Configuration vs release metadata

PC Inform uses **two separate configuration layers**:

### 1. `appsettings.json` (deployment / administrator)

Location on end-user machines:

```
%LOCALAPPDATA%\PCInform\appsettings.json
```

Purpose:

- Branding (`application`, `support`)
- Feature toggles (`features`)
- **Update source configuration** (`update.enabled`, `update.versionUrl`, `features.checkUpdates`)

This file is intended for **IT administrators** or deployment packaging — not for casual end-user editing.

Example administrator settings for managed update checks:

```json
{
  "features": {
    "checkUpdates": true
  },
  "update": {
    "enabled": true,
    "versionUrl": "https://your-domain.example/pcinform/latest/version.json"
  }
}
```

Public repository defaults in [appsettings.example.json](../appsettings.example.json) keep updates and Atera detection **disabled** for safe out-of-the-box use.

End users normally should **not** change update settings unless instructed by their IT team.

### 2. `version.json` (release metadata)

Hosted remotely (for example on GitHub Releases assets or a static web server).

Purpose:

- Advertise the latest published version
- Provide download URL for the installer
- Optional release notes (PL/EN)

Example: [docs/version.example.json](version.example.json)

The application compares `version.json` against the running assembly version on startup **only when enabled in `appsettings.json`**.

## GitHub Releases workflow

### Build artifacts (do not commit to source tree)

Never commit compiled binaries to the repository:

- `PCInform.exe`
- `PCInform-Setup.exe`
- publish output folders

These belong on **GitHub Releases**, not in git history.

### Recommended release steps

1. Update version in `PCInform/PCInform.csproj`.
2. Update `CHANGELOG.md`.
3. Publish the application:

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

4. Build the installer:

   ```powershell
   iscc installer\PCInform.iss
   ```

5. Create a GitHub Release tagged `vX.Y.Z`.

6. Attach release assets:

   - `PCInform-Setup.exe` (recommended for end users)
   - Optional: `PCInform.exe` ZIP for portable use
   - `version.json` pointing to the release download URL

7. Host or update `version.json` so `update.versionUrl` in deployed `appsettings.json` references the current release metadata.

### `version.json` download URL

Point `downloadUrl` to the GitHub Release asset, for example:

```
https://github.com/TimeWizard007/pcinform/releases/download/v1.0.1/PCInform-Setup.exe
```

The application opens this URL in the browser when the user accepts an update — it does **not** auto-install.

## Installer upgrades

The Inno Setup script:

- Installs per-user to `%LOCALAPPDATA%\PCInform\`
- Preserves existing `appsettings.json` if present
- Replaces the application executable

Administrators can preconfigure `appsettings.json` before distributing the installer or via deployment tooling.
