// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MsiSweep.Model;

/// <summary>
/// Compares <see cref="Package"/> instances by their code and their local package path.
/// </summary>
/// <remarks>
/// The local package paths are compared case-insensitively. All other properties are ignored.
/// </remarks>
public sealed class PackageEqualityComparer : IEqualityComparer<Package>
{
    /// <summary>
    /// The string comparison used for the local package paths.
    /// </summary>
    public const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// Gets the default instance of the <see cref="PackageEqualityComparer"/>.
    /// </summary>
    /// <value>A shared <see cref="PackageEqualityComparer"/> instance.</value>
    public static IEqualityComparer<Package> Default { get; } = new PackageEqualityComparer();

    /// <inheritdoc/>
    public bool Equals(Package? x, Package? y)
    {
        return x?.Code == y?.Code
            && string.Equals(x?.LocalPackage.Value, y?.LocalPackage.Value, Comparison);
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Package obj)
    {
        return HashCode.Combine(
            obj.Code,
            string.GetHashCode(obj.LocalPackage.Value, Comparison));
    }
}
