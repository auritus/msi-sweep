// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.ComponentModel;
using MsiSweep.Model;
using MsiSweep.Msi.Api;

using static MsiSweep.Msi.Api.NativeMethods;

namespace MsiSweep.Inventory;

/// <summary>
/// Enumerates the registered products and patches through the Windows Installer API.
/// </summary>
/// <param name="userLookup">The lookup that resolves the SIDs of the packages to user names.</param>
/// <param name="registryLookup">The lookup that provides the registry paths of the packages.</param>
public sealed class MsiApiPackageEnumerator(
    IUserLookup userLookup,
    IRegistryLookup registryLookup) : IPackageEnumerator
{
    /// <inheritdoc/>
    /// <exception cref="Win32Exception">The Windows Installer API fails to enumerate the packages.</exception>
    public IEnumerable<Package> EnumerateInstalledPackages(PackageKind kind) => kind switch
    {
        PackageKind.Patch => EnumeratePatches(),
        PackageKind.Product => EnumerateProducts(),
        _ => throw new ArgumentException($"An invalid PackageKind has been provided: {kind}", nameof(kind)),
    };

    #region Implementations
    /// <summary>
    /// The Everyone SID, which the MSI enumeration functions interpret as all users.
    /// </summary>
    /// <remarks>
    /// See https://learn.microsoft.com/en-us/windows/win32/api/msi/nf-msi-msienumproductsexa#parameters
    /// </remarks>
    private const string AllUsersSid = "s-1-1-0";

    private IEnumerable<Package> EnumerateProducts()
    {
        var inlineProduct = new InlineGuid();

        for (var index = 0U; ; ++index)
        {
            var sid = new Sid();
            var result = MsiEnumProductsEx(
                szProductCode: null,
                szUserSid: AllUsersSid,
                dwContext: InstallContext.All,
                dwIndex: index,
                szInstalledProductCode: inlineProduct,
                pdwInstalledContext: out var context,
                szSid: sid.Buffer,
                pcchSid: ref sid.Length);

            if (result == ErrorCode.NoMoreItems)
            {
                yield break;
            }

            if (result != ErrorCode.Success)
            {
                throw new Win32Exception((int)result, $"Failed to enumerate products. Error code: {result}");
            }

            var product = inlineProduct.ToGuid();
            var sidAsString = sid.ToString();
            var localPackage = NativeHelper.GetProductInfo(product, sidAsString, context, "LocalPackage");

            yield return new Package(
                Kind: PackageKind.Product,
                Code: product,
                Name: NativeHelper.GetProductInfo(product, sidAsString, context, "ProductName") ?? string.Empty,
                User: userLookup.GetUserName(sidAsString),
                RegistryKey: registryLookup.GetPackagePath(PackageKind.Product, product, sidAsString),
                LocalPackage: new LocalPackagePath(localPackage),
                Source: EnumerationSource.Msi
            );
        }
    }

    private IEnumerable<Package> EnumeratePatches()
    {
        var inlinePatch = new InlineGuid();
        var inlineProduct = new InlineGuid();

        for (var index = 0U; ; ++index)
        {
            var sid = new Sid();
            var result = MsiEnumPatchesEx(null, AllUsersSid, InstallContext.All, PatchState.All,
                index,
                inlinePatch,
                inlineProduct,
                out var context,
                sid.Buffer,
                ref sid.Length);

            if (result == ErrorCode.NoMoreItems)
            {
                yield break;
            }

            if (result != ErrorCode.Success)
            {
                throw new Win32Exception((int)result, $"Failed to enumerate patches. Error code: {result}");
            }

            var patch = inlinePatch.ToGuid();
            var product = inlineProduct.ToGuid();
            var sidAsString = sid.ToString();
            var localPackage = NativeHelper.GetPatchInfo(patch, product, sidAsString, context, "LocalPackage");

            yield return new Package(
                Kind: PackageKind.Patch,
                Code: patch,
                Name: NativeHelper.GetPatchInfo(patch, product, sidAsString, context, "DisplayName") ?? string.Empty,
                User: userLookup.GetUserName(sidAsString),
                RegistryKey: registryLookup.GetPackagePath(PackageKind.Patch, patch, sidAsString),
                LocalPackage: new LocalPackagePath(localPackage),
                Source: EnumerationSource.Msi
            );
        }
    }
    #endregion
}
