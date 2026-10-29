// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;
using static MsiSweep.Tests.FakePackageEnumerator;

namespace MsiSweep.Tests;

public sealed class PackageEqualityTests
{
    [Fact]
    public void LocalPackagePath_IgnoresCase()
    {
        var lower = new LocalPackagePath(@"C:\Windows\Installer\abc.msi");
        var upper = new LocalPackagePath(@"C:\WINDOWS\INSTALLER\ABC.MSI");

        Assert.Equal(lower, upper);
        Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
    }

    [Fact]
    public void LocalPackagePath_ExpandsEnvironmentVariables()
    {
        var expanded = Path.Join(Environment.GetEnvironmentVariable("WINDIR"), "Installer", "abc.msi");

        Assert.Equal(new LocalPackagePath(expanded), new LocalPackagePath(@"%WINDIR%\Installer\abc.msi"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void LocalPackagePath_EmptyEqualsDefault(string? path)
    {
        var localPackagePath = new LocalPackagePath(path);

        Assert.Equal(default, localPackagePath);
        Assert.Equal(string.Empty, localPackagePath.Value);
    }

    [Fact]
    public void PackageEqualityComparer_SameCodeAndPathIgnoringCase_AreEqual()
    {
        var code = Guid.NewGuid();
        var x = CreatePackage(@"C:\Windows\Installer\abc.msi", PackageKind.Product, code);
        var y = CreatePackage(@"C:\WINDOWS\INSTALLER\ABC.MSI", PackageKind.Product, code);

        Assert.True(PackageEqualityComparer.Default.Equals(x, y));
        Assert.Equal(PackageEqualityComparer.Default.GetHashCode(x), PackageEqualityComparer.Default.GetHashCode(y));
    }

    [Fact]
    public void PackageEqualityComparer_DifferentCode_AreNotEqual()
    {
        var x = CreatePackage(@"C:\Windows\Installer\abc.msi");
        var y = CreatePackage(@"C:\Windows\Installer\abc.msi");

        Assert.False(PackageEqualityComparer.Default.Equals(x, y));
    }

    [Fact]
    public void PackageEqualityComparer_DifferentPath_AreNotEqual()
    {
        var code = Guid.NewGuid();
        var x = CreatePackage(@"C:\Windows\Installer\abc.msi", code: code);
        var y = CreatePackage(@"C:\Windows\Installer\def.msi", code: code);

        Assert.False(PackageEqualityComparer.Default.Equals(x, y));
    }
}
