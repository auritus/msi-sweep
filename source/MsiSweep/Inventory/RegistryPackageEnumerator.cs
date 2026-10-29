// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using Microsoft.Win32;
using MsiSweep.Extensions;
using MsiSweep.Model;
using MsiSweep.Msi.Api;

namespace MsiSweep.Inventory;

/// <summary>
/// Enumerates the registered products and patches by reading the Windows Installer <c>UserData</c> registry key.
/// </summary>
/// <remarks>
/// This finds registrations that the Windows Installer API does not report, for example ones left behind
/// by an incomplete uninstall.
/// </remarks>
/// <param name="userLookup">The lookup that resolves the SIDs of the packages to user names.</param>
/// <param name="registryLookup">The lookup that provides the registry paths of the packages.</param>
public sealed class RegistryPackageEnumerator(
    IUserLookup userLookup,
    IRegistryLookup registryLookup) : IPackageEnumerator
{
    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">The <c>UserData</c> key does not exist or cannot be read.</exception>
    public IEnumerable<Package> EnumerateInstalledPackages(PackageKind kind)
    {
        return kind switch
        {
            PackageKind.Patch => EnumeratePatches(),
            PackageKind.Product => EnumerateProducts(),
            _ => throw new ArgumentException($"An invalid PackageKind has been provided: {kind}", nameof(kind)),
        };
    }

    #region Implementation
    private RegistryKey OpenInstallerUserDataPath()
    {
        using var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

        return hklm.OpenSubKey(registryLookup.InstallerUserDataPath)
            ?? throw new InvalidOperationException($@"HKLM\{registryLookup.InstallerUserDataPath} not found or not readable.");
    }

    private IEnumerable<Package> EnumerateProducts()
    {
        using var userData = OpenInstallerUserDataPath();

        foreach (var sid in userData.GetSubKeyNames())
        {
            using var products = userData.OpenSubKey($@"{sid}\Products");

            if (products is not null)
            {
                foreach (var product in products.GetSubKeyNames())
                {
                    using var properties = products.OpenSubKey($@"{product}\InstallProperties");

                    if (properties is null || !Guid.TryParsePackedString(product, out var code))
                    {
                        continue;
                    }

                    var kind = PackageKind.Product;

                    yield return new Package(
                        Kind: kind,
                        Code: code,
                        Name: GetProductName(code, sid) ?? string.Empty,
                        User: userLookup.GetUserName(sid),
                        RegistryKey: registryLookup.GetPackagePath(kind, code, sid),
                        LocalPackage: new LocalPackagePath(properties.GetValue("LocalPackage") as string),
                        Source: EnumerationSource.Registry
                    );
                }
            }
        }
    }

    private IEnumerable<Package> EnumeratePatches()
    {
        using var userData = OpenInstallerUserDataPath();

        foreach (var sid in userData.GetSubKeyNames())
        {
            using var patches = userData.OpenSubKey($@"{sid}\Patches");

            var productNameByPatch = GetPatchProductNames(userData, sid);

            if (patches is not null)
            {
                foreach (var patch in patches.GetSubKeyNames())
                {
                    using var properties = patches.OpenSubKey(patch);

                    if (properties is null || !Guid.TryParsePackedString(patch, out var code))
                    {
                        continue;
                    }

                    var kind = PackageKind.Patch;
                    var name = productNameByPatch.TryGetValue(patch, out var productName)
                        ? productName ?? string.Empty
                        : string.Empty;

                    yield return new Package(
                        Kind: kind,
                        Code: code,
                        Name: name,
                        User: userLookup.GetUserName(sid),
                        RegistryKey: registryLookup.GetPackagePath(kind, code, sid),
                        LocalPackage: new LocalPackagePath(properties.GetValue("LocalPackage") as string),
                        Source: EnumerationSource.Registry
                    );
                }
            }
        }
    }

    private string? GetProductName(Guid product, string sid)
    {
        if (sid.Equals(userLookup.LocalSystemSid, StringComparison.OrdinalIgnoreCase))
        {
            return GetProductNameCore(product, null, InstallContext.Machine);
        }

        return GetProductNameCore(product, sid, InstallContext.UserManaged)
            ?? GetProductNameCore(product, sid, InstallContext.UserUnmanaged);

        static string? GetProductNameCore(Guid product, string? sid, InstallContext context)
        {
            return NativeHelper.GetProductInfo(product, sid, context, "ProductName");
        }
    }

    private static Dictionary<string, string?> GetPatchProductNames(RegistryKey userData, string sid)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        using var products = userData.OpenSubKey($@"{sid}\Products");

        if (products is null)
        {
            return result;
        }

        foreach (var product in products.GetSubKeyNames())
        {
            using var productPatches = products.OpenSubKey($@"{product}\Patches");

            if (productPatches is null || !Guid.TryParsePackedString(product, out var code))
            {
                continue;
            }

            foreach (var patch in productPatches.GetSubKeyNames())
            {
                using var patchKey = productPatches.OpenSubKey(patch);

                result.TryAdd(patch, patchKey?.GetValue("DisplayName") as string);
            }
        }

        return result;
    }
    #endregion
}
