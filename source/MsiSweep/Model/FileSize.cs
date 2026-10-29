// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Represents the size of a file or directory, in bytes.
/// </summary>
/// <param name="Value">The size in bytes.</param>
public readonly record struct FileSize(long Value) : IComparable<FileSize>
{
    /// <summary>
    /// Converts the size to a human-readable string with two decimal places and a binary unit.
    /// </summary>
    /// <param name="provider">The provider used to format the number, or <see langword="null"/> to use the current culture.</param>
    /// <returns>
    /// The size in the largest unit from B to TB that keeps the value at least at 1, for example <c>1.50 MB</c>.
    /// </returns>
    public readonly string ToHumanizedString(IFormatProvider? provider = null)
    {
        var unit = 0;
        var size = Value * 1.0;

        for (; size >= 1024 && unit < 4; ++unit, size /= 1024.0)
        {
            ;
        }

        var unitText = unit switch
        {
            0 => "B",
            1 => "KB",
            2 => "MB",
            3 => "GB",
            _ => "TB"
        };

        return string.Create(provider, $"{size:0.00} {unitText}");
    }

    #region Operators
    /// <summary>
    /// Converts a size in bytes to a <see cref="FileSize"/>.
    /// </summary>
    /// <param name="value">The size in bytes.</param>
    public static implicit operator FileSize(long value) => new(value);

    /// <summary>
    /// Determines whether one <see cref="FileSize"/> is less than another.
    /// </summary>
    /// <param name="x">The first size to compare.</param>
    /// <param name="y">The second size to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="x"/> is less than <paramref name="y"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator < (FileSize x, FileSize y) => x.Value < y.Value;

    /// <summary>
    /// Determines whether one <see cref="FileSize"/> is greater than another.
    /// </summary>
    /// <param name="x">The first size to compare.</param>
    /// <param name="y">The second size to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="x"/> is greater than <paramref name="y"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator > (FileSize x, FileSize y) => x.Value > y.Value;
    #endregion

    #region Overrides
    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Converts the size to its string representation in bytes.
    /// </summary>
    /// <returns>The size in bytes, without a unit. Use <see cref="ToHumanizedString"/> for a human-readable form.</returns>
    public override string ToString() => Value.ToString();
    #endregion

    #region IComparable<FileSize>
    /// <inheritdoc/>
    public int CompareTo(FileSize other) => Value.CompareTo(other.Value);
    #endregion
}
