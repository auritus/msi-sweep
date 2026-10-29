// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;

namespace MsiSweep.Inventory;

/// <summary>
/// Enumerates the products and patches registered with Windows Installer.
/// </summary>
public interface IPackageEnumerator
{
    /// <summary>
    /// Enumerates the registered packages of the specified kind for all users and install contexts.
    /// </summary>
    /// <param name="kind">The kind of packages to enumerate.</param>
    /// <returns>An enumerable collection of the registered packages.</returns>
    /// <exception cref="ArgumentException"><paramref name="kind"/> is not a valid <see cref="PackageKind"/>.</exception>
    IEnumerable<Package> EnumerateInstalledPackages(
        PackageKind kind);
}
