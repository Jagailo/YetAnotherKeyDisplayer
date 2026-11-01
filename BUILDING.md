# Building Yet Another Key Displayer

## Requirements

- Windows 10 or Windows 11.
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (version 18.x), any edition including Community, with:
  - the **.NET desktop development** workload;
  - the **Desktop development with C++** workload (it provides the MSVC v145 x86/x64 build tools and the Windows SDK);
  - the **.NET Framework 4.8.1 targeting pack** and **.NET Framework 4.8.1 SDK** individual components;
  - the **C++/CLI support for v145 build tools** individual component, if the C++ project fails to build with an error about `/clr` or C++/CLI.
- [Inno Setup](https://jrsoftware.org/isdl.php) 6.7 or later, only for creating the installer.

The C++ project uses the v145 platform toolset. Visual Studio 2022 can build it only after retargeting the project to v143; do not commit that change.

## Repository layout

| Path | Contents |
| --- | --- |
| `YAKD/` | WPF application (C#, .NET Framework 4.8.1). |
| `RTSSCustomOSD/` | `RTSSSharedMemoryNET.dll`, a C++/CLI wrapper over the RTSS shared memory (LGPL-3.0). |
| `Installer/` | Inno Setup script. `Installer/Languages/` holds installer translations that are not shipped with Inno Setup. |
| `version.json`, `config.json` | Read by installed copies from the `master` branch: the latest version for the update notification and the statistics endpoint. |

## Building

### Visual Studio

1. Open `YAKD.slnx`.
2. Select the **Release** configuration and the **Any CPU** platform.
3. Run **Build > Build Solution**. The application is written to `YAKD\bin\Release\`.

The solution has only the **Any CPU** platform on purpose. `RTSSSharedMemoryNET.dll` is a mixed-mode Win32 assembly, so the C++ project is always built for Win32 and `YAKD.exe` is always x86.

The application manifest requires administrator rights (the keyboard hook has to work in elevated games). To debug, start Visual Studio as administrator, otherwise it offers to restart with elevated rights.

Debug builds do not send the installation statistics.

### Command line

Run in **Developer PowerShell for VS 2026** from the repository root:

```powershell
msbuild YAKD.slnx -restore -p:Configuration=Release "-p:Platform=Any CPU"
```

`dotnet build` cannot be used: C++/CLI projects are built only by the Visual Studio MSBuild.

## Creating the installer

1. Build the solution in **Release | Any CPU**.
2. Compile `Installer\YAKD.iss`: open it in the Inno Setup Compiler and press **Compile**, or run

   ```powershell
   & "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" Installer\YAKD.iss
   ```

3. The installer is written to `Installer\Output\YAKD_<version>_installer.exe`.

The product name, version, publisher and copyright are read from `YAKD.exe`, so the script normally does not need to be changed for a new release.

The installer:

- installs into `Program Files (x86)\Yet Another Key Displayer` or into the folder of the previous installation;
- checks that .NET Framework 4.8.1 is installed and offers to open its download page otherwise;
- when installed over 1.5 or older, removes the uninstaller, shortcuts and "Apps & features" entry of the old NSIS installer.

`AppId` in the script must never change: it is shared with the old NSIS installer and identifies the application for upgrades.

To add an installer language, add a line to the `[Languages]` section. Languages shipped with Inno Setup are referenced as `compiler:Languages\<Name>.isl`, other translations are put into `Installer\Languages\`.

## Adding an interface language

1. Copy `YAKD\Localization\Strings.resx` to `Strings.<culture>.resx` (for example, `Strings.uk.resx`) and translate the values. Keep the `{0}` placeholders.
2. Add `new LanguageModel("<culture>", "<native name>")` to `SupportedLanguages` in `YAKD\Localization\LocalizationManager.cs`, following the ordering described there.
3. Add the matching installer language, if Inno Setup has one.
4. Update the language lists in `README.md` and `CHANGELOG.md`.

## Release checklist

1. Set the new version in:
   - `YAKD/YAKD.csproj`: `Version`, `AssemblyVersion`, `FileVersion`;
   - `YAKD/Properties/app.manifest`: `assemblyIdentity version`;
   - `RTSSCustomOSD/RTSSSharedMemoryNET.rc`: `FILEVERSION`, `PRODUCTVERSION`, `FileVersion`, `ProductVersion`;
   - `RTSSCustomOSD/AssemblyInfo.cpp`: `AssemblyVersion`, `AssemblyFileVersion`;
   - `version.json`: `latestVersion` (for example, `"1.6.0"`), which is checked by 1.6 and newer, and `version`, a legacy number checked by 1.5 and older. `version` must grow with every release, for example `1600` for 1.6 and `1610` for 1.6.1.
2. Do not change `Company` in `YAKD/YAKD.csproj`: it is part of the settings path, and users would lose their settings after the update.
3. In `CHANGELOG.md`, replace `Unreleased` with the release date and check the comparison link at the bottom.
4. In `README.md`, update the download link and the installer size.
5. Rebuild the solution in **Release | Any CPU** and create the installer.
6. Test the installer over the previous version: the settings are kept, the shortcut works, both display modes work, the application can be uninstalled.
7. Commit the changes to `dev`, create a tag without the `v` prefix (for example, `1.6`) and push it.
8. Publish a GitHub release for the tag and attach the installer with the file name used in `README.md`.
9. Merge `dev` into `master` and push. Installed copies read `version.json` from `master`, so do this only after the release is published.
