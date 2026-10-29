// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Inventory;
using MsiSweep.Model;

using static MsiSweep.Tests.FakePackageEnumerator;

namespace MsiSweep.Tests;

public sealed class InstallerInventoryTests
{
    [Fact]
    public void EnumerateOrphanedPackageFiles_ReturnsOnlyUnregisteredFiles()
    {
        using var directory = new TestDirectory();
        var registered = directory.CreateFile("registered.msi");
        var orphaned = directory.CreateFile("orphaned.msi");
        var inventory = new InstallerInventory(new FakePackageEnumerator(CreatePackage(registered)), directory.Path);

        var orphans = inventory.EnumerateOrphanedPackageFiles().ToArray();

        var orphan = Assert.Single(orphans);
        Assert.Equal(new LocalPackagePath(orphaned), orphan.LocalPackagePath);
    }

    [Fact]
    public void EnumerateOrphanedPackageFiles_IgnoresCaseOfRegisteredPath()
    {
        using var directory = new TestDirectory();
        var registered = directory.CreateFile("Registered.MSI");
        var inventory = new InstallerInventory(
            new FakePackageEnumerator(CreatePackage(registered.ToLowerInvariant())),
            directory.Path);

        Assert.Empty(inventory.EnumerateOrphanedPackageFiles());
    }

    [Fact]
    public void EnumerateOrphanedPackageFiles_ConsidersProductsAndPatches()
    {
        using var directory = new TestDirectory();
        var product = directory.CreateFile("product.msi");
        var patch = directory.CreateFile("patch.msp");
        var inventory = new InstallerInventory(
            new FakePackageEnumerator(
                CreatePackage(product, PackageKind.Product),
                CreatePackage(patch, PackageKind.Patch)),
            directory.Path);

        Assert.Empty(inventory.EnumerateOrphanedPackageFiles());
    }

    [Fact]
    public void EnumerateOrphanedPackageFiles_ReturnsLargestFirst()
    {
        using var directory = new TestDirectory();
        directory.CreateFile("small.msi", size: 1);
        directory.CreateFile("large.msi", size: 100);
        directory.CreateFile("medium.msp", size: 10);
        var inventory = new InstallerInventory(new FakePackageEnumerator(), directory.Path);

        var names = inventory.EnumerateOrphanedPackageFiles().Select(file => file.FileName);

        Assert.Equal(["large.msi", "medium.msp", "small.msi"], names);
    }

    [Fact]
    public void EnumeratePackageFiles_ReturnsOnlyPackageFilesInTopDirectory()
    {
        using var directory = new TestDirectory();
        directory.CreateFile("product.msi");
        directory.CreateFile("patch.MSP");
        directory.CreateFile("readme.txt");
        directory.CreateFile("product.msi.bak");
        Directory.CreateDirectory(Path.Join(directory.Path, "folder.msi"));
        Directory.CreateDirectory(Path.Join(directory.Path, "sub"));
        File.WriteAllBytes(Path.Join(directory.Path, "sub", "nested.msi"), [0]);
        var inventory = new InstallerInventory(new FakePackageEnumerator(), directory.Path);

        var names = inventory.EnumeratePackageFiles(directory.Path).Select(file => file.FileName).Order();

        Assert.Equal(["patch.MSP", "product.msi"], names);
    }

    [Fact]
    public void EnumeratePackageFiles_SkipsExcludedFiles()
    {
        using var directory = new TestDirectory();
        var excluded = directory.CreateFile("excluded.msi");
        directory.CreateFile("included.msi");
        var inventory = new InstallerInventory(new FakePackageEnumerator(), directory.Path);

        var files = inventory.EnumeratePackageFiles(directory.Path, new HashSet<LocalPackagePath> { new(excluded) });

        Assert.Equal("included.msi", Assert.Single(files).FileName);
    }

    [Fact]
    public void EnumeratePackageFiles_MissingDirectory_Throws()
    {
        using var directory = new TestDirectory();
        var inventory = new InstallerInventory(new FakePackageEnumerator(), directory.Path);

        Assert.Throws<DirectoryNotFoundException>(
            () => inventory.EnumeratePackageFiles(Path.Join(directory.Path, "missing")).ToArray());
    }

    [Fact]
    public void Packages_RemovesDuplicates()
    {
        var code = Guid.NewGuid();
        var inventory = new InstallerInventory(new FakePackageEnumerator(
            CreatePackage(@"C:\Windows\Installer\a.msi", code: code),
            CreatePackage(@"C:\WINDOWS\INSTALLER\A.MSI", code: code),
            CreatePackage(@"C:\Windows\Installer\b.msi")));

        Assert.Equal(2, inventory.Packages.Count);
    }

    [Fact]
    public void Packages_EnumeratesOnce()
    {
        var enumerator = new FakePackageEnumerator(CreatePackage(@"C:\Windows\Installer\a.msi"));
        var inventory = new InstallerInventory(enumerator);

        _ = inventory.Packages;
        _ = inventory.Packages;

        // One call for the products and one for the patches.
        Assert.Equal(2, enumerator.CallCount);
    }
}
