// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.IO.Enumeration;
using MsiSweep.Model;
using MsiSweep.Msi.Api;

namespace MsiSweep.Inventory;

/// <summary>
/// Provides the packages registered with Windows Installer and the package files cached in the
/// Windows Installer directory.
/// </summary>
/// <param name="enumerator">The enumerator that supplies the registered products and patches.</param>
/// <param name="installerDirectory">
/// The directory to use for installer lookup or <see langword="null"/>, to use the directory
/// the OS uses by default.
/// </param>
public sealed class InstallerInventory(
    IPackageEnumerator enumerator,
    string? installerDirectory = null)
{
    /// <summary>
    /// Gets the full path of the Windows Installer directory, <c>%WINDIR%\Installer</c> or the directory
    /// passed to the constructor.
    /// </summary>
    /// <value>The full path of the directory that holds the cached <c>.msi</c> and <c>.msp</c> files.</value>
    public string Directory { get; } = installerDirectory ??
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            "Installer");

    /// <summary>
    /// Gets the registered products and patches.
    /// </summary>
    /// <value>
    /// The registered products followed by the registered patches. Duplicates, as defined by
    /// <see cref="PackageEqualityComparer"/>, are removed.
    /// </value>
    /// <remarks>
    /// The packages are enumerated once, on the first access to this property.
    /// </remarks>
    public IReadOnlyList<Package> Packages => _packages.Value;

    /// <summary>
    /// Determines whether a Windows Installer installation is currently in progress.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <c>Global\_MSIExecute</c> mutex exists; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsInstallationInProgress()
    {
        try
        {
            if (Mutex.TryOpenExisting(@"Global\_MSIExecute", out var result))
            {
                result.Dispose();
                return true;
            }

            return false;
        }
        catch (UnauthorizedAccessException)
        {
            // The mutex exists, but cannot be opened; treat it as an installation in progress.
            return true;
        }
    }

    /// <summary>
    /// Enumerates the package files in <see cref="Directory"/> that no registered package references.
    /// </summary>
    /// <returns>The orphaned package files, largest first.</returns>
    public IEnumerable<LocalPackageFile> EnumerateOrphanedPackageFiles()
    {
        return EnumeratePackageFiles(
                Directory,
                Packages.Select(package => package.LocalPackage).ToHashSet())
            .OrderByDescending(file => file.Size);
    }

    /// <summary>
    /// Enumerates the <c>.msi</c> and <c>.msp</c> files in a directory, without subdirectories.
    /// </summary>
    /// <param name="directory">The directory to search.</param>
    /// <param name="excludes">The paths of the files to skip, or <see langword="null"/> to include all files.</param>
    /// <returns>The package files. The sequence is evaluated lazily.</returns>
    /// <exception cref="DirectoryNotFoundException"><paramref name="directory"/> does not exist.</exception>
    public IEnumerable<LocalPackageFile> EnumeratePackageFiles(
        string directory,
        IReadOnlySet<LocalPackagePath>? excludes = null)
    {
        return new FileSystemEnumerable<LocalPackageFile>(
            directory,
            (ref e) =>
            {
                return new LocalPackageFile(
                    localPackagePath: new LocalPackagePath(e.ToFullPath()),
                    size: e.Length,
                    getPackageName: static (localPackagePath) =>
                    {
                        try
                        {
                            return NativeHelper.GetPackageName(localPackagePath.Value) ?? string.Empty;
                        }
                        catch (InvalidOperationException)
                        {
                            return string.Empty;
                        }
                    });
            },
            new EnumerationOptions()
            {
                AttributesToSkip = FileAttributes.Directory,
                IgnoreInaccessible = true,
                RecurseSubdirectories = false
            })
        {
            ShouldIncludePredicate = (ref e) =>
                (
                    e.FileName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase) ||
                    e.FileName.EndsWith(".msp", StringComparison.OrdinalIgnoreCase)
                ) && excludes?.Contains(new LocalPackagePath(e.ToFullPath())) is not true
        };
    }

    #region Implementation
    private readonly Lazy<IReadOnlyList<Package>> _packages = new(() =>
        [
            .. enumerator
                .EnumerateInstalledPackages(PackageKind.Product)
                .Union(enumerator.EnumerateInstalledPackages(PackageKind.Patch), PackageEqualityComparer.Default)
        ]);
    #endregion
}
