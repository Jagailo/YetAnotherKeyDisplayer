# Changelog

All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [1.6] - 2026-10-30

### Added
- Interface in 12 languages: English, German, Spanish, French, Italian, Polish, Portuguese (Brazil), Turkish, Russian, Japanese, Chinese (Simplified) and Korean ([#38](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/38), [#44](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/44)).
- Language selection button. Until a language is chosen, the interface follows the Windows display language.
- "Exit" item in the context menu of the tray icon ([#42](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/42)).
- Version, publisher and copyright information in the file properties of all binaries.
- Per-Monitor V2 DPI awareness for sharp rendering on high-DPI and mixed-DPI displays.
- Build and release instructions in [BUILDING.md](BUILDING.md).

### Changed
- Maximum display delay increased to 7 seconds ([#45](https://github.com/Jagailo/YetAnotherKeyDisplayer/issues/45)).
- New Inno Setup installer replaces the NSIS one: modern wizard, 12 languages, .NET Framework 4.8.1 check and complete "Apps & features" information. Installing over 1.5 keeps the installation folder and removes the old uninstaller and shortcuts.
- Punctuation keys are shown with their unshifted US layout characters (`;` `'` `,` `.` `=` `/` `` ` ``) instead of a mix of shifted and unshifted ones. The extra key next to the left Shift on ISO keyboards is shown as `\` instead of `/`.
- Numpad keys are named "NumPad", and the short name option shows "Num 5" instead of "Pad 5".
- The update check compares full version numbers.
- The project has been migrated to the SDK-style project format and the `.slnx` solution format.
- The license is now the standard MIT License without additional terms.

### Fixed
- Mouse wheel scrolling was not detected on high-resolution mice and touchpads.
- Keyboard and mouse hooks of a hidden YAKD window stayed active, which could slow down input after the window had been shown and hidden several times.
- An RTSS error could open a message box while a key was held down, blocking keyboard and mouse input.
- Display delay did not work correctly: a released key disappeared early on any other key event and stayed on screen next to a newly pressed key. Now a released key is shown for the whole delay or until the next key press.

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

[1.6]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.5...1.6
[1.5]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.4...1.5
[1.4]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.3...1.4
[1.3]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/1.2.1...1.3
[1.2.1]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.2...1.2.1
[1.2]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.1...v1.2
[1.1]: https://github.com/Jagailo/YetAnotherKeyDisplayer/compare/v1.0...v1.1
[1.0]: https://github.com/Jagailo/YetAnotherKeyDisplayer/releases/tag/v1.0
