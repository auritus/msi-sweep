// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;

namespace MsiSweep.Inventory;

/// <summary>
/// Provides the registry paths of Windows Installer registrations.
/// </summary>
public interface IRegistryLookup
{
    /// <summary>
    /// Gets the path of the Windows Installer's <c>UserData</c> key.
    /// </summary>
    string InstallerUserDataPath { get; }

    /// <summary>
    /// Gets the path of the registry key that holds the install properties of a package.
    /// </summary>
    /// <param name="kind">The kind of the package.</param>
    /// <param name="code">The product code or patch code of the package.</param>
    /// <param name="sid">
    /// The SID of the user the package is registered for, or <see langword="null"/> for a per-machine package.
    /// </param>
    /// <returns>The path of the registry key, relative to <c>HKEY_LOCAL_MACHINE</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="kind"/> is not a valid <see cref="PackageKind"/>.</exception>
    string GetPackagePath(
        PackageKind kind,
        Guid code,
        string? sid);
}
