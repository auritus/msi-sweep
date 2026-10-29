// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.ComponentModel;
using System.Reflection;
using System.Security;
using MsiSweep;
using MsiSweep.Extensions;
using MsiSweep.Inventory;
using MsiSweep.Model;
using MsiSweep.Output;

var options = Options.FromCommandLine(args, Console.Error);
var version = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion ?? "unknown";

Console.WriteLine($"MsiSweep {version}");
Console.WriteLine();

if (options is null or { ShowHelp: true })
{
    var writer = options is null
        ? Console.Error
        : Console.Out;

    writer.WriteLine("Usage: MsiSweep --list | --list-orphaned | --list-dangling | --move-orphaned <backupDir> | --restore <restoreDir> | --version | --help");
    writer.WriteLine("  --list                            List orphaned files and dangling registrations.");
    writer.WriteLine("  --list-orphaned                   List orphaned .msi/.msp files (not registered).");
    writer.WriteLine("  --list-dangling                   List dangling registrations (registered, cached package missing).");
    writer.WriteLine("  --move-orphaned <backupDir>       Same report as --list-orphaned, then move orphaned files to <backupDir>.");
    writer.WriteLine("  --restore <restoreDir>            Move all .msi/.msp files from <restoreDir> back to the Windows Installer directory.");
    writer.WriteLine("  --version                         Print the version and exit.");
    writer.WriteLine("  --help                            Print this help and exit.");

    // Explicit --help is a success; missing or invalid arguments are a usage error.
    return options is null ? 2 : 0;
}

try
{
    return Run(options);
}
catch (Exception ex) when (
    ex is Win32Exception
    or InvalidOperationException
    or IOException
    or UnauthorizedAccessException
    or SecurityException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static int Run(Options options)
{
    if (options.ShowVersion)
    {
        return 0;
    }

    if (InstallerInventory.IsInstallationInProgress())
    {
        try
        {
            if (options.MoveTargetDir is not null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine();
                Console.Error.WriteLine(
                    """
                    An installation is currently in progress, which might cause --move-orphaned to perform incorrectly.
                    Wait for the installation to complete and run this tool again.
                    """);
                return 1;
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine("An installation is currently in progress, so the results might be inaccurate.");
            Console.WriteLine();
        }
        finally
        {
            Console.ResetColor();
        }
    }

    var inventory = CreateInventory();
    var exitCode = 0;

    if (options.RestoreSourceDir is { } sourceDir)
    {
        exitCode = Restore(inventory, sourceDir);
    }

    if (options.ListDangling)
    {
        DanglingPackagesTable.Print(inventory);
    }

    if (options.ListOrphaned || options.MoveTargetDir is not null)
    {
        const string status = "Collecting orphaned packages ...";

        Console.Write(status);

        var orphans = inventory
            .EnumerateOrphanedPackageFiles()
            .ToArray();

        Console.Write($"\r{new string(' ', status.Length)}\r");
        OrphanedPackagesTable.Print(orphans);

        if (options.MoveTargetDir is { } targetDir)
        {
            exitCode = MoveOrphans(orphans, targetDir);
        }
    }

    return exitCode;
}

static int Restore(InstallerInventory inventory, string sourceDir)
{
    Console.WriteLine();
    Console.WriteHeader("Restore Packages");
    Console.WriteLine();

    var files = inventory
        .EnumeratePackageFiles(sourceDir)
        .ToArray();

    if (files.Length == 0)
    {
        Console.WriteLine($"The directory \"{sourceDir}\" does not contain any .msi or .msp files to restore.");
        Console.WriteLine();
        return 0;
    }

    var result = LocalPackageFileMover.MovePackageFiles(
        files,
        inventory.Directory,
        ProgressReporter.CreateConsoleReporter("Restored", inventory.Directory));

    Console.WriteLine();
    Console.WriteLine($"Restored {result.Succeeded} of {files.Length} files to \"{inventory.Directory}\"");
    Console.WriteLine();

    if (result.Failed > 0)
    {
        Console.Error.WriteLine($"{result.Failed} files could not be restored.");
        return 1;
    }

    return 0;
}

static int MoveOrphans(LocalPackageFile[] orphans, string targetDir)
{
    Console.WriteLine();
    Console.WriteHeader("Move Orphaned Packages");
    Console.WriteLine();

    if (orphans.Length == 0)
    {
        Console.WriteLine("No orphaned packages detected.");
        Console.WriteLine();
        return 0;
    }

    Console.WriteLine($"Starting cleanup of {orphans.Length} orphaned installer files.");

    var result = LocalPackageFileMover.MovePackageFiles(
        orphans,
        targetDir,
        ProgressReporter.CreateConsoleReporter("Moved", targetDir));

    Console.WriteLine();
    Console.WriteLine($"Moved {result.Succeeded} of {orphans.Length} files to \"{targetDir}\"");
    Console.WriteLine();

    if (result.Failed > 0)
    {
        Console.Error.WriteLine($"{result.Failed} files could not be moved.");
        return 1;
    }

    return 0;
}

static InstallerInventory CreateInventory()
{
    var userLookup = new UserLookup();
    var registryLookup = new RegistryLookup(userLookup);
    var enumerator = new CompositePackageEnumerator(
        new MsiApiPackageEnumerator(userLookup, registryLookup),
        new RegistryPackageEnumerator(userLookup, registryLookup));

    return new InstallerInventory(enumerator);
}
