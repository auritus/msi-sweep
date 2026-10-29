# MsiSweep

MsiSweep cleans up the `%WINDIR%\Installer` directory by moving orphaned `.msi` and `.msp` files
into a backup directory.

Windows Installer keeps a copy of every installed package in this directory, and over time it can grow by
many gigabytes. Deleting files from it by hand is risky: Windows Installer may need a file again to repair,
update or uninstall a product. MsiSweep checks each file against the [MSI API](https://learn.microsoft.com/en-us/windows/win32/api/msi/)
and the registry, and moves only the files that neither source references.

The tool can also:

- List the orphaned files before moving them.
- List registered products and patches whose cached package is missing. Repairing, updating or uninstalling them may fail.
- Restore moved files into `%WINDIR%\Installer`.

## How it works

Each cached package has a random file name, and its path is registered as `LocalPackage`.
The tool collects these paths from two sources:

- the MSI API (`MsiEnumProductsEx`, `MsiEnumPatchesEx`), covering all users and install contexts
- the registry, under `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\<SID>\Products\<code>\InstallProperties` and `...\UserData\<SID>\Patches\<code>`

A file in `%WINDIR%\Installer` is reported as orphaned only if neither source references it.
If the MSI API enumeration fails, the tool aborts instead of working with an incomplete list.

The tool can print two different reports:

- **Orphaned files**: size, file name and product name. The name is read from the package itself
  (`Property.ProductName` for `.msi`, `MsiPatchMetadata.DisplayName` for `.msp`, falling back to the summary "Subject").
- **Dangling registrations**: products and patches that are still registered but whose cached file is gone,
  with kind, product/patch code, missing file path and the registry key holding the `LocalPackage` value. Repair, uninstall and patching of these may fail.
  Restore the package with `msiexec /fv "<original.msi>"` using the same version of the original package.

## Download

Download `MsiSweep.exe` from the [releases](../../releases) page. The executable is not code-signed yet,
so Windows SmartScreen may warn about an unknown publisher. Each release also contains `MsiSweep.exe.sha256`.
Verify the download before running it:

```powershell
Get-FileHash MsiSweep.exe -Algorithm SHA256
```

The printed hash must match the one in `MsiSweep.exe.sha256`.

Alternatively, check the [Build](#build) section to compile and run the tool yourself.

## Usage

The tool requires an elevated prompt to access the installations of all users. The executable requests administrator rights through its manifest.
While a Windows Installer installation is in progress, `--move-orphaned` refuses to move any files, because packages
that are being installed may not be registered yet. The other commands print a warning and continue.

| Command | Description |
| --- | --- |
| `--list` | Print the full report. Changes nothing. |
| `--list-orphaned` | Print only the orphaned files. Changes nothing. |
| `--list-dangling` | Print only the dangling registrations. Changes nothing. |
| `--move-orphaned <backupDir>` | Print the orphaned files, then move them to `<backupDir>`. Nothing is deleted. |
| `--restore <restoreDir>` | Move all `.msi` and `.msp` files from `<restoreDir>` back to `%WINDIR%\Installer`. Cannot be combined with `--move-orphaned`. |
| `--version` | Print the version and exit. |
| `--help` | Print the usage and exit. |

Exit codes: `0` success, `1` an error occurred, `--move-orphaned` was refused because an installation is in progress, or at least one file could not be moved or restored, `2` missing or invalid arguments.

Run `--list` first and review the result. Keep the backup folder until you have confirmed that repairs,
updates and uninstalls of your installed software still work, then delete it yourself.
To undo a move, run `--restore <backupDir>`. It moves every `.msi` and `.msp` file in that folder into
`%WINDIR%\Installer`, so use it only on a backup folder created by `--move-orphaned`. Files that already exist in
`%WINDIR%\Installer` are not overwritten and are reported as failed.

## Build

### Requirements

- .NET 10 SDK
- For native AOT: Visual Studio with the "Desktop development with C++" workload

### Compile

```bash
dotnet build MsiSweep.slnx
```

### Publish

Native AOT is enabled by default to create a small executable with a lower memory footprint
and no dependencies.

```bash
dotnet publish source/MsiSweep -r win-x64
```

A managed application, which requires the .NET 10 runtime, can be created with the following command:

```bash
dotnet publish source/MsiSweep -r win-x64 -p:PublishAot=false
```

The executable is written to `.\artifacts\publish\MsiSweep\release_win-x64\MsiSweep.exe`.

### Tests

The tests use xUnit v3 on Microsoft.Testing.Platform (enabled in `global.json`).

```bash
dotnet test --solution MsiSweep.slnx
```
