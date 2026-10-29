// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Extensions;
using MsiSweep.Model;

namespace MsiSweep.Output;

internal static class OrphanedPackagesTable
{
    public static void Print(IReadOnlyList<LocalPackageFile> packageFiles)
    {
        Console.WriteHeader("Orphaned Packages Summary");

        var table = new Table<LocalPackageFile>([
            Column.Create("Size", (LocalPackageFile file) => file.Size.ToHumanizedString(), ColumnAlignment.Right),
            Column.Create("File", (LocalPackageFile file) => file.FileName),
            Column.Create("Package", (LocalPackageFile file) => file.PackageName),
        ]);

        table.Print(packageFiles, new Margin(4));

        Console.WriteLine();
        Console.WriteLine($"Orphaned: {packageFiles.Count} files, {GetTotalFileSize(packageFiles).ToHumanizedString()}");
        Console.WriteLine();
    }

    #region Implementation
    private static FileSize GetTotalFileSize(IReadOnlyList<LocalPackageFile> files)
    {
        var size = 0L;

        foreach (var file in files)
        {
            size += file.Size.Value;
        }

        return new FileSize(size);
    }
    #endregion
}
