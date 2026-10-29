// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Inventory;
using MsiSweep.Model;

namespace MsiSweep.Tests;

/// <summary>
/// Returns a fixed list of packages and counts the enumerations.
/// </summary>
internal sealed class FakePackageEnumerator(params Package[] packages) : IPackageEnumerator
{
    public int CallCount { get; private set; }

    public IEnumerable<Package> EnumerateInstalledPackages(PackageKind kind)
    {
        CallCount++;
        return packages.Where(package => package.Kind == kind);
    }

    public static Package CreatePackage(
        string localPackage,
        PackageKind kind = PackageKind.Product,
        Guid? code = null) =>
        new(
            Kind: kind,
            Code: code ?? Guid.NewGuid(),
            Name: "Test",
            User: "S-1-5-18",
            RegistryKey: string.Empty,
            LocalPackage: new LocalPackagePath(localPackage),
            Source: EnumerationSource.Msi);
}
