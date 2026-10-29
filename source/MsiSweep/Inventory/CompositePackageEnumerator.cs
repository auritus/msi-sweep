// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;

namespace MsiSweep.Inventory;

/// <summary>
/// Initializes a new <see cref="CompositePackageEnumerator"/>, which creates a union
/// from two package sources, using the <see cref="PackageEqualityComparer"/> to determine
/// equality.
/// </summary>
/// <param name="first">The first package enumerator.</param>
/// <param name="second">The second package enumerator.</param>
internal sealed class CompositePackageEnumerator(
    IPackageEnumerator first,
    IPackageEnumerator second) : IPackageEnumerator
{
    public IEnumerable<Package> EnumerateInstalledPackages(PackageKind kind)
    {
        return first
            .EnumerateInstalledPackages(kind)
            .Union(second.EnumerateInstalledPackages(kind), PackageEqualityComparer.Default);
    }
}
