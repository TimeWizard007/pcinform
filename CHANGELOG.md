# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
- Polish and English UI with per-user language persistence
- Configurable `appsettings.json` for branding, support contact, features, and updates
- Computer, user, TeamViewer, and optional Atera information collection
- Copy to clipboard and report-problem email workflow
- Outlook Classic COM draft support with `mailto:` fallback
- Optional remote update check via `version.json`
- Per-user installer script (Inno Setup) for `%LOCALAPPDATA%\PCInform\`

[1.1.0]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.1.0
[1.0.0]: https://github.com/TimeWizard007/pcinform/releases/tag/v1.0.0
