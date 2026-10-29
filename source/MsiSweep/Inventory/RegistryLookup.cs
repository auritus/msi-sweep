// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Extensions;
using MsiSweep.Model;

namespace MsiSweep.Inventory;

/// <summary>
/// Provides the registry paths of Windows Installer registrations under the <c>UserData</c> key.
/// </summary>
/// <param name="userLookup">The lookup that provides the SID used for per-machine registrations.</param>
public sealed class RegistryLookup(IUserLookup userLookup) : IRegistryLookup
{
    /// <inheritdoc/>
    public string InstallerUserDataPath => @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData";

    /// <inheritdoc/>
    public string GetPackagePath(PackageKind kind, Guid code, string? sid)
    {
        var guid = code.ToPackedString();

        sid ??= userLookup.LocalSystemSid;

        return kind switch
        {
            PackageKind.Product => $@"{InstallerUserDataPath}\{sid}\Products\{guid}\InstallProperties",
            PackageKind.Patch => $@"{InstallerUserDataPath}\{sid}\Patches\{guid}",
            _ => throw new ArgumentException($"An invalid PackageKind has been provided: {kind}", nameof(kind)),
        };
    }
}
