// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace MsiSweep.Model;

/// <summary>
/// Represents the normalized path of a cached Windows Installer package.
/// </summary>
/// <remarks>
/// Paths are compared case-insensitively.
/// </remarks>
/// <param name="path">
/// The path of the package. Environment variables are expanded, and the path is converted to a full path.
/// A <see langword="null"/> or empty path results in an empty <see cref="Value"/>.
/// </param>
public readonly struct LocalPackagePath(string? path) : IEquatable<LocalPackagePath>
{
    /// <summary>
    /// Gets the normalized full path of the package.
    /// </summary>
    /// <value>The full path, or <see cref="string.Empty"/> if no path was specified.</value>
    // Coalesced so default(LocalPackagePath) behaves like an empty path instead of exposing null.
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Gets a value that indicates whether the package file exists.
    /// </summary>
    /// <value><see langword="true"/> if the file at <see cref="Value"/> exists; otherwise, <see langword="false"/>.</value>
    public bool Exists => File.Exists(Value);

    #region Overrides
    /// <summary>
    /// Returns the normalized full path of the package.
    /// </summary>
    /// <returns>The value of <see cref="Value"/>.</returns>
    public override string ToString() => Value;

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified object is a <see cref="LocalPackagePath"/> with the same path, ignoring case.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="LocalPackagePath"/> with the same path;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        LocalPackagePath other => Equals(other),
        _ => false,
    };
    #endregion

    #region Operators
    /// <summary>
    /// Determines whether two <see cref="LocalPackagePath"/> instances have the same path, ignoring case.
    /// </summary>
    /// <param name="left">The first path to compare.</param>
    /// <param name="right">The second path to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same path;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(LocalPackagePath left, LocalPackagePath right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="LocalPackagePath"/> instances have different paths, ignoring case.
    /// </summary>
    /// <param name="left">The first path to compare.</param>
    /// <param name="right">The second path to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have different paths;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(LocalPackagePath left, LocalPackagePath right)
    {
        return !(left == right);
    }
    #endregion

    #region IEquatable<LocalPackagePath>
    /// <summary>
    /// Determines whether this instance and another <see cref="LocalPackagePath"/> have the same path, ignoring case.
    /// </summary>
    /// <param name="other">The path to compare with this instance.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="other"/> has the same path; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(LocalPackagePath other) => Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    #endregion

    #region Implementation
    private readonly string? _value = string.IsNullOrEmpty(path)
        ? null
        : Path.GetFullPath(
            Environment.ExpandEnvironmentVariables(path));
    #endregion
}
