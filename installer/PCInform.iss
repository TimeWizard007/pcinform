; PC Inform — per-machine end-user installer (administrator required)
; Build PCInform.exe first, then compile this script with Inno Setup 6.
; Optional: place appsettings.json next to PCInform-Setup.exe to seed global config on first install.

#define AppName "PC Inform"
#define AppExe "PCInform.exe"
#define PublishDir "..\PCInform\bin\Release\net8.0-windows\win-x64\publish"
#define ExampleConfig "..\appsettings.example.json"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#AppName}
AppVersion=1.2.1
AppVerName=PC Inform v1.2.1-dev
AppPublisher=pcinform
DefaultDirName={autopf}\PCInform
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
OutputBaseFilename=PCInform-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Dirs]
Name: "{commonappdata}\PCInform"; Permissions: users-modify

[Files]
Source: "{#PublishDir}\{#AppExe}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "appsettings.json,PCInform.Configurator.exe"
Source: "{src}\appsettings.json"; DestDir: "{commonappdata}\PCInform"; DestName: "appsettings.json"; Flags: onlyifdoesntexist external skipifsourcedoesntexist
Source: "{#ExampleConfig}"; DestDir: "{commonappdata}\PCInform"; DestName: "appsettings.json"; Flags: onlyifdoesntexist

[Icons]
Name: "{commonprograms}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent
