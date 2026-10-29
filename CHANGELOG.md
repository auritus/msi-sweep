# Changelog

All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-10-29

### Added

- `--list`: report orphaned files and dangling registrations.
- `--list-orphaned`: list `.msi` and `.msp` files in `%WINDIR%\Installer` that no installed product or patch references.
  Registered packages are collected from the MSI API and from the `UserData` registry key.
- `--list-dangling`: list registered products and patches whose cached package file is missing.
- `--move-orphaned <backupDir>`: list the orphaned files, then move them to `<backupDir>`. Nothing is deleted.
- `--restore <restoreDir>`: move all `.msi` and `.msp` files from `<restoreDir>` back to `%WINDIR%\Installer`.
  It cannot be combined with `--move-orphaned`.
- `--help`: print the usage.
- `--version`: print the version.
- `--move-orphaned` refuses to move files while a Windows Installer installation is in progress
  (`Global\_MSIExecute` mutex). The list commands print a warning instead.
- Exit codes: `0` success, `1` an error occurred, `--move-orphaned` was refused because an installation is in
  progress, or at least one file could not be moved or restored, `2` missing or invalid arguments.
- Native AOT build for `win-x64`.
