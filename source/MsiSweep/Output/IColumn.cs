// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Output;

/// <summary>
/// Defines a column of a table that shows elements of type <typeparamref name="TElement"/>.
/// </summary>
/// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
public interface IColumn<TElement>
{
    /// <summary>
    /// Gets the header text of the column.
    /// </summary>
    /// <value>The text shown in the header row.</value>
    string Header { get; }

    /// <summary>
    /// Gets the alignment of the cell text.
    /// </summary>
    /// <value>One of the enumeration values that specifies the alignment.</value>
    ColumnAlignment Alignment { get; }

    /// <summary>
    /// Gets the cell text of the column for the specified element.
    /// </summary>
    /// <param name="element">The element to get the cell text for.</param>
    /// <returns>The cell text.</returns>
    string GetValueAsString(TElement element);
}
