// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Tests;

public sealed class OptionsTests
{
    [Fact]
    public void NoArguments_ReturnsNull()
    {
        Assert.Null(Options.FromCommandLine([]));
    }

    [Fact]
    public void List_EnablesOrphanedAndDangling()
    {
        var options = Options.FromCommandLine(["--list"]);

        Assert.NotNull(options);
        Assert.True(options.ListOrphaned);
        Assert.True(options.ListDangling);
        Assert.Null(options.MoveTargetDir);
    }

    [Fact]
    public void ListOrphaned_EnablesOnlyOrphaned()
    {
        var options = Options.FromCommandLine(["--list-orphaned"]);

        Assert.NotNull(options);
        Assert.True(options.ListOrphaned);
        Assert.False(options.ListDangling);
    }

    [Fact]
    public void ListDangling_EnablesOnlyDangling()
    {
        var options = Options.FromCommandLine(["--list-dangling"]);

        Assert.NotNull(options);
        Assert.False(options.ListOrphaned);
        Assert.True(options.ListDangling);
    }

    [Fact]
    public void ListOrphanedAndListDangling_EnableBoth()
    {
        var options = Options.FromCommandLine(["--list-orphaned", "--list-dangling"]);

        Assert.NotNull(options);
        Assert.True(options.ListOrphaned);
        Assert.True(options.ListDangling);
    }

    [Fact]
    public void MoveOrphaned_SetsTargetDirectory()
    {
        var options = Options.FromCommandLine(["--move-orphaned", @"D:\Backup"]);

        Assert.NotNull(options);
        Assert.Equal(@"D:\Backup", options.MoveTargetDir);
        Assert.False(options.ListOrphaned);
        Assert.False(options.ListDangling);
    }

    [Fact]
    public void Restore_SetsSourceDirectory()
    {
        var options = Options.FromCommandLine(["--restore", @"D:\Backup"]);

        Assert.NotNull(options);
        Assert.Equal(@"D:\Backup", options.RestoreSourceDir);
        Assert.Null(options.MoveTargetDir);
        Assert.False(options.ListOrphaned);
        Assert.False(options.ListDangling);
    }

    [Fact]
    public void MoveOrphanedAndRestore_ReturnNull()
    {
        using var error = new StringWriter();

        Assert.Null(Options.FromCommandLine(["--move-orphaned", @"D:\Backup", "--restore", @"D:\Backup"], error));
        Assert.Contains("cannot be combined", error.ToString());
    }

    [Fact]
    public void Help_SetsShowHelp()
    {
        var options = Options.FromCommandLine(["--help"]);

        Assert.NotNull(options);
        Assert.True(options.ShowHelp);
    }

    [Fact]
    public void Version_SetsShowVersion()
    {
        var options = Options.FromCommandLine(["--version"]);

        Assert.NotNull(options);
        Assert.True(options.ShowVersion);
        Assert.False(options.ListOrphaned);
        Assert.False(options.ListDangling);
        Assert.Null(options.MoveTargetDir);
    }

    [Theory]
    [InlineData("--bogus")]
    [InlineData("--move-orphaned")]
    [InlineData("--list", "--bogus")]
    [InlineData("--move-orphaned", "--list")]
    [InlineData("--restore")]
    [InlineData("--restore", "--list")]
    [InlineData("list")]
    public void InvalidArguments_ReturnNullAndReportTheArgument(params string[] args)
    {
        using var error = new StringWriter();

        Assert.Null(Options.FromCommandLine(args, error));
        Assert.Contains("Unknown option or missing parameter", error.ToString());
    }
}
