#define AppName "ACC Connector"
#define AppVersion "1.0"
#define AppPublisher "Ilpo Ruotsalainen"
#define AppURL "https://github.com/lonemeow/acc-connector"
#define AppExeName "ACC Connector.exe"

[Setup]
AppId={{51604B81-B031-42E3-B3D1-35440A387EBD}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={autopf}\{#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
OutputBaseFilename=ACC-Connector-Setup-{#AppVer}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "client-gui\bin\Release\net8.0-windows10.0.19041.0\publish\ACC Connector.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "client-gui\bin\Release\net8.0-windows10.0.19041.0\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "client-gui\bin\Release\net8.0-windows10.0.19041.0\publish\*.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "x64\Release\client-hooks.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "minhook\LICENSE.txt"; DestDir: "{app}"; DestName: "THIRD_PARTY_LICENSES.txt"; Flags: ignoreversion

[Registry]
Root: HKCR; Subkey: "acc-connect"; ValueType: "string"; ValueData: "URL:Custom Protocol"; Flags: uninsdeletekey
Root: HKCR; Subkey: "acc-connect"; ValueType: "string"; ValueName: "URL Protocol"; ValueData: ""
Root: HKCR; Subkey: "acc-connect\DefaultIcon"; ValueType: "string"; ValueData: "{app}\{#AppExeName},0"
Root: HKCR; Subkey: "acc-connect\shell\open\command"; ValueType: "string"; ValueData: """{app}\{#AppExeName}"" ""%1"""

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
