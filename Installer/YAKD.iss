; Inno Setup 6.7+ script. Build the solution in Release|Any CPU before compiling, see BUILDING.md.

#define AppExeName "YAKD.exe"
#define BinDir SourcePath + "..\YAKD\bin\Release"
#define AppExePath BinDir + "\" + AppExeName
#define AppName GetStringFileInfo(AppExePath, PRODUCT_NAME)
#define AppVersion GetStringFileInfo(AppExePath, PRODUCT_VERSION)
#define AppPublisher GetStringFileInfo(AppExePath, COMPANY_NAME)
#define AppCopyright GetStringFileInfo(AppExePath, LEGAL_COPYRIGHT)
#define AppUrl "https://github.com/Jagailo/YetAnotherKeyDisplayer"

; Registry key of the legacy NSIS installer (versions up to 1.5) that shared the same application GUID.
#define LegacyUninstallKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\{00A267B0-8858-4C17-812B-1903AF25C494}"

[Setup]
AppId={{00A267B0-8858-4C17-812B-1903AF25C494}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}/issues
AppUpdatesURL={#AppUrl}/releases
AppCopyright={#AppCopyright}
DefaultDirName={code:GetDefaultDirName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
PrivilegesRequired=admin
MinVersion=10.0
OutputDir=Output
OutputBaseFilename=YAKD_{#AppVersion}_installer
SetupIconFile=..\YAKD\Images\Icons\main-icon.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
WizardStyle=modern
Compression=lzma2/max
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
; Not shipped with Inno Setup 6.7, taken from https://github.com/jrsoftware/issrc/tree/main/Files/Languages.
Name: "chinesesimplified"; MessagesFile: "Languages\ChineseSimplified.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[CustomMessages]
DotNetMissing=Yet Another Key Displayer requires .NET Framework 4.8.1.%n%nDo you want to open the download page now?

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#BinDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml,Xceed.Wpf.AvalonDock*"; Flags: ignoreversion recursesubdirs
Source: "..\LICENSE"; DestDir: "{app}"; DestName: "LICENSE.txt"; Flags: ignoreversion
Source: "..\THIRD-PARTY-NOTICES.md"; DestDir: "{app}"; DestName: "THIRD-PARTY-NOTICES.txt"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
; The application requires elevation, so it has to be started through ShellExecute.
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[Code]
function IsDotNet481Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and (Release >= 533320);
end;

function InitializeSetup: Boolean;
var
  ErrorCode: Integer;
begin
  Result := IsDotNet481Installed;
  if not Result and (SuppressibleMsgBox(CustomMessage('DotNetMissing'), mbError, MB_YESNO, IDNO) = IDYES) then
    ShellExec('open', 'https://dotnet.microsoft.com/download/dotnet-framework/net481', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;

function GetDefaultDirName(Param: String): String;
begin
  if not RegQueryStringValue(HKLM, '{#LegacyUninstallKey}', 'InstallDirectory', Result) or (Result = '') then
    Result := ExpandConstant('{autopf}\{#AppName}');
end;

procedure RemoveLegacyInstallation;
var
  InstallDir, StartMenuFolder: String;
begin
  if not RegKeyExists(HKLM, '{#LegacyUninstallKey}') then
    Exit;

  if RegQueryStringValue(HKLM, '{#LegacyUninstallKey}', 'StartmenuFolder', StartMenuFolder) and (StartMenuFolder <> '') then
    DelTree(ExpandConstant('{commonprograms}\') + StartMenuFolder, True, True, True);

  if RegQueryStringValue(HKLM, '{#LegacyUninstallKey}', 'InstallDirectory', InstallDir) and (InstallDir <> '') then
    DeleteFile(AddBackslash(InstallDir) + 'Uninstall.exe');

  DeleteFile(ExpandConstant('{commondesktop}\YAKD.lnk'));
  RegDeleteKeyIncludingSubkeys(HKLM, '{#LegacyUninstallKey}');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    RemoveLegacyInstallation;
end;
