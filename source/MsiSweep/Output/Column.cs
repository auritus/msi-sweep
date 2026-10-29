// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Output;

/// <summary>
/// Provides factory methods for table columns.
/// </summary>
public static class Column
{
    /// <summary>
    /// Creates a column that converts each element to a string with the specified function.
    /// </summary>
    /// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
    /// <param name="header">The header text of the column.</param>
    /// <param name="toString">A function that returns the cell text for an element.</param>
    /// <param name="alignment">The alignment of the cell text. Headers are always left-aligned.</param>
    /// <returns>The new column.</returns>
    public static IColumn<TElement> Create<TElement>(
        string header,
        Func<TElement, string> toString,
        ColumnAlignment alignment = ColumnAlignment.Left)
            where TElement : notnull
        => new Column<TElement>(header, toString, alignment);

    /// <summary>
    /// Creates a column that selects a value from each element and shows the result of its
    /// <see cref="object.ToString"/> method.
    /// </summary>
    /// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
    /// <typeparam name="TValue">The type of the value shown in the column.</typeparam>
    /// <param name="header">The header text of the column.</param>
    /// <param name="toValue">A function that returns the value for an element.</param>
    /// <param name="alignment">The alignment of the cell text. Headers are always left-aligned.</param>
    /// <returns>The new column.</returns>
    public static IColumn<TElement> Create<TElement, TValue>(
        string header,
        Func<TElement, TValue> toValue,
        ColumnAlignment alignment = ColumnAlignment.Left)
            where TValue : notnull
            where TElement : notnull
        => new Column<TElement>(
            header,
            row => toValue(row).ToString() ?? string.Empty,
            alignment);
}