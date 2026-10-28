# Changelog

All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [1.6] - Unreleased

### Added
- Interface language selection with support for 12 languages (translations are in progress).
- Version, publisher and copyright information in the file properties of all binaries.
- Per-Monitor V2 DPI awareness.

### Changed
- New Inno Setup installer with a modern wizard, localized pages, a .NET Framework 4.8.1 check and complete "Apps & features" information.
- The project has been migrated to the SDK-style project format and the `.slnx` solution format.
- The license is now the standard MIT License without additional terms.

## [1.5] - 2026-06-08

### Added
- Ignoring of IME keys ([#43](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/43)).
- Ability to minimize the application completely to the tray ([#31](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/31)).
- Option to replace arrow key names with symbols.
- Tooltip text for the tray icon.

### Changed
- Maximum display delay increased to 5 seconds ([#32](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/32)).
- Improved RTSS performance ([#30](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/30)).
- Polished application texts and layout.
- Icons are embedded directly, the SharpVectors dependency has been removed.
- The license has been changed from GPLv3 to modified MIT.

## [1.4] - 2024-05-05

### Added
- Option to display keys only while they are pressed ([#20](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/20)).
- Tray icon ([#23](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/23)).
- Option to ignore the difference between left and right modifier keys ([#13](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/13)).
- Short names for numpad keys.
- Click-through window setting ([#19](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/19)).
- Installer.

### Changed
- Replaced the color picker with a more practical one.

### Fixed
- RTSS overlay did not work if RTSS was started before YAKD ([#17](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/17)).
- Size and position of the Key Displayer were not saved after applying the default settings.

## [1.3] - 2022-04-10

### Added
- Key press display delay.
- Middle mouse button tracking.
- Export and import of settings.

## [1.2.1] - 2020-12-25

### Added
- Ability to fix the Key Displayer window in place.

## [1.2] - 2020-04-29

### Added
- Mouse buttons tracking.
- Keys alignment.
- Buttons to increase and decrease the font size.
- Notification about new versions.

### Changed
- Improved color picker.
- Opacity is now set in percent.
- Updated to .NET Framework 4.8.

## [1.1] - 2018-07-17

### Added
- Full support of RivaTuner Statistics Server (RTSS) with a custom OSD.

### Fixed
- The Space key no longer toggles the focused check box.

## [1.0] - 2018-07-09

- First release.

[1.6]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.5...dev
[1.5]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.4...1.5
[1.4]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.3...1.4
[1.3]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.2.1...1.3
[1.2.1]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.2...1.2.1
[1.2]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.1...v1.2
[1.1]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.0...v1.1
[1.0]: https://github.com/Jagailo/YetAnotherKeyDisplayer/releases/tag/v1.0
