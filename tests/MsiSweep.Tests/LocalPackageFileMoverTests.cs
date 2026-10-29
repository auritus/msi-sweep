// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Inventory;
using MsiSweep.Model;

namespace MsiSweep.Tests;

public sealed class LocalPackageFileMoverTests
{
    [Fact]
    public void MovePackageFiles_CreatesTargetDirectoryAndMovesFiles()
    {
        using var source = new TestDirectory();
        using var target = new TestDirectory();

        var targetDirectory = Path.Join(target.Path, "backup");
        var files = new[] { CreateFile(source, "a.msi"), CreateFile(source, "b.msp") };

        var result = LocalPackageFileMover.MovePackageFiles(files, targetDirectory);

        Assert.Equal(new MovePackageFilesResult(Succeeded: 2, Failed: 0), result);
        Assert.True(File.Exists(Path.Join(targetDirectory, "a.msi")));
        Assert.True(File.Exists(Path.Join(targetDirectory, "b.msp")));
        Assert.False(File.Exists(files[0].LocalPackagePath.Value));
    }

    [Fact]
    public void MovePackageFiles_DoesNotOverwriteExistingFiles()
    {
        using var source = new TestDirectory();
        using var target = new TestDirectory();

        var existing = target.CreateFile("a.msi", size: 5);
        var files = new[] { CreateFile(source, "a.msi", size: 10), CreateFile(source, "b.msi") };
        var reports = new List<MoveLocalPackageFilesProgress>();

        var result = LocalPackageFileMover.MovePackageFiles(files, target.Path, reports.Add);

        Assert.Equal(new MovePackageFilesResult(Succeeded: 1, Failed: 1), result);
        Assert.Equal(5, new FileInfo(existing).Length);
        Assert.True(File.Exists(files[0].LocalPackagePath.Value));
        Assert.IsType<IOException>(reports[0].Exception, exactMatch: false);
        Assert.Null(reports[1].Exception);
    }

    [Fact]
    public void MovePackageFiles_ReportsProgressForEachFile()
    {
        using var source = new TestDirectory();
        using var target = new TestDirectory();
        var files = new[] { CreateFile(source, "a.msi"), CreateFile(source, "b.msi"), CreateFile(source, "c.msi") };
        var reports = new List<MoveLocalPackageFilesProgress>();

        LocalPackageFileMover.MovePackageFiles(files, target.Path, reports.Add);

        Assert.Equal([0, 1, 2], reports.Select(report => report.Index));
        Assert.All(reports, report => Assert.Equal(3, report.TotalCount));
        Assert.Equal(files, reports.Select(report => report.LocalPackageFile));
    }

    private static LocalPackageFile CreateFile(TestDirectory directory, string name, int size = 1) =>
        new(
            new LocalPackagePath(directory.CreateFile(name, size)),
            size,
            getPackageName: _ => string.Empty);
}
