# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.1] - 2026-05-27

### Added

- Final application icon for the main app, configurator, installer, and shortcuts

### Fixed

- SemVer update comparison for prerelease tags (e.g. `-dev`, `-rc1`)

### Changed

- Update and network status footer tooltips (current vs new version; Internet/DNS details)

## [1.2.0] - 2026-06-13

### Added

- Network status footer indicator (`features.showNetworkStatus`) with Internet and DNS tooltips
- Discreet update footer indicator (`update.showFooterIndicator`) when a newer version is available
- Semantic version comparison for update checks (including prerelease tags such as `-dev` and `-rc1`)
- Diagnostic logging to `C:\ProgramData\PCInform\Logs\PCInform.log`
- Automatic schema upgrade for missing config fields (`showNetworkStatus`, `includeNetworkStatus`, `showFooterIndicator`) with safe defaults

### Changed

- Update check is informational only: background check, footer **⬆️** indicator, and About notice — no startup Yes/No dialog and no automatic install
- Improved network status and update indicator tooltips (Internet/DNS details; current vs new version)

## [1.1.0] - 2026-06-13

### Added

- Configurable PC Inform application with machine-wide `appsettings.json` in `C:\ProgramData\PCInform\`
- Per-machine all-users installer (`PCInform-Setup.exe`) for `C:\Program Files\PCInform\`
- Optional standalone administrator configurator (`PCInform.Configurator.exe`) distributed separately
- Separate **Display** (`features.show*`) and **Report** (`report.include*`) settings for UI vs clipboard/email content
- `report` section in JSON configuration with safe upgrade for existing deployments
- Friendly labels in PC Inform Configurator (Application, Support, Features, Report, Update tabs)
- MIT license section and improved About dialog (Polish and English)

### Changed

- Improved user display name detection (local account, domain, profile name, username fallback)
- Improved main window label/value row alignment
- Documentation updates for configuration, installer, release process, and Display vs Report behaviour

## [1.0.0] - 2026-05-28

### Added

- Initial open-source release of **PC Inform**
- Polish and English UI with per-user language preference (`%APPDATA%\PCInform\settings.json`)
- Configurable `appsettings.json` for branding, support contact, features, and updates
- Computer, user, TeamViewer, and optional Atera information collection
- Copy to clipboard and report-problem email workflow
- Outlook Classic COM draft support with `mailto:` fallback
- Optional remote update check via `version.json`

[1.2.1]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.2.1
[1.2.0]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.2.0
[1.1.0]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.1.0
[1.0.0]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.0.0
