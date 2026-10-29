// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Output;

/// <summary>
/// Represents a table column that converts each element to a string with a function.
/// </summary>
/// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
/// <param name="header">The header text of the column.</param>
/// <param name="toString">A function that returns the cell text for an element.</param>
/// <param name="alignment">The alignment of the cell text.</param>
internal sealed class Column<TElement>(
    string header,
    Func<TElement, string> toString,
    ColumnAlignment alignment) : IColumn<TElement>
{
    /// <inheritdoc/>
    public string Header { get; } = header;

    /// <inheritdoc/>
    public string GetValueAsString(TElement element) => toString(element);

    /// <inheritdoc/>
    public ColumnAlignment Alignment { get; } = alignment;
}
