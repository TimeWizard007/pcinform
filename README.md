# Dane komputera

Windows desktop application for IT Solution Service Desk end users.

## Requirements

- Windows 10 or Windows 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for building only)

The Release build is **self-contained** — end users do not need to install .NET separately.

## Icon

Place `icon.ico` in the `DaneKomputera` project folder. It is used as:

- executable icon
- window icon
- taskbar icon

Replace the placeholder `icon.ico` with your IT Solution branding icon before publishing.

## Debug run

```powershell
dotnet run --project DaneKomputera\DaneKomputera.csproj
```

## Release publish (single-file .exe)

```powershell
dotnet publish DaneKomputera\DaneKomputera.csproj `
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
DaneKomputera\bin\Release\net8.0-windows\win-x64\publish\Dane-komputera.exe
```

## Distribution

Copy `Dane-komputera.exe` to end users. No installation step is required.

## User settings

Language preference is saved to:

```
%AppData%\DaneKomputera\settings.json
```

## Features

- Polish (default) and English UI
- Outlook Classic draft email (fallback to `mailto:`)
- Device type detection (Desktop / Laptop / Virtual Machine)
- TeamViewer launch button when installed
- Atera Agent detection (included in clipboard/email reports only)
- Copy data and report problem with prefilled email template
- No administrator privileges required
