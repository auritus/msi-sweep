// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Represents a package file in the Windows Installer directory.
/// </summary>
/// <param name="localPackagePath">
/// The full path of the file.
/// </param>
/// <param name="getPackageName">
/// A callback to obtain the package name. Since this operation might
/// be relatively expensive, it is only invoked on demand once and the
/// result is stored in a backing field.
/// </param>
/// <param name="size">The size of the file.</param>
public sealed class LocalPackageFile(
    LocalPackagePath localPackagePath,
    FileSize size,
    Func<LocalPackagePath, string> getPackageName)
{
    /// <summary>
    /// Gets the full path of the package file.
    /// </summary>
    public LocalPackagePath LocalPackagePath { get; } = localPackagePath;

    /// <summary>
    /// Gets the size of the file.
    /// </summary>
    public FileSize Size { get; } = size;

    /// <summary>
    /// Gets the product name or patch display name read from the file,
    /// or <see cref="string.Empty"/> if it cannot be read.
    /// </summary>
    public string PackageName
    {
        get
        {
            if (_packageName is { } packageName)
            {
                return packageName;
            }

            return _packageName = _getPackageName(LocalPackagePath);
        }
    }

    /// <summary>
    /// Gets the file name and extension of the package file.
    /// </summary>
    /// <value>The file name of <see cref="LocalPackagePath"/>, without the directory.</value>
    public string FileName => Path.GetFileName(LocalPackagePath.Value);

    #region Implementation
    private readonly Func<LocalPackagePath, string> _getPackageName = getPackageName;
    private string? _packageName;
    #endregion
}
