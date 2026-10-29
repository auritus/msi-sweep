// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Represents a product or patch registered with Windows Installer.
/// </summary>
/// <param name="Kind">The kind of the package.</param>
/// <param name="Code">The product code or patch code.</param>
/// <param name="Name">The product name or patch display name, or <see cref="string.Empty"/> if it is unknown.</param>
/// <param name="User">The name of the user the package is registered for, or a placeholder for per-machine packages.</param>
/// <param name="RegistryKey">The path of the registration key, relative to <c>HKEY_LOCAL_MACHINE</c>.</param>
/// <param name="LocalPackage">The path of the cached package file in the Windows Installer directory.</param>
/// <param name="Source">Where the registration was found.</param>
public sealed record Package(
    PackageKind Kind,
    Guid Code,
    string Name,
    string User,
    string RegistryKey,
    LocalPackagePath LocalPackage,
    EnumerationSource Source
);
