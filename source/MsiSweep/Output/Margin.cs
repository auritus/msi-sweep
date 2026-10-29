// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Output;

/// <summary>
/// Represents the margin of a table.
/// </summary>
/// <param name="value">The number of columns to indent by.</param>
public readonly struct Margin(
    int value)
{
    /// <summary>
    /// Gets the number of columns to indent by.
    /// </summary>
    /// <value>The width of the margin in console columns.</value>
    public int Value { get; } = value;
}
