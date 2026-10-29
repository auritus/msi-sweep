// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Output;

/// <summary>
/// Prints elements to the console as blocks, with one line per column.
/// </summary>
/// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
/// <param name="columns">
/// The columns of the table. The first column is printed as the title of each block, without its header.
/// </param>
public sealed class VerticalTable<TElement>(
    IColumn<TElement>[] columns)
{
    /// <summary>
    /// Prints one block for each element.
    /// </summary>
    /// <param name="elements">The elements to print.</param>
    /// <param name="leftMargin">The horizontal offset for each line.</param>
    /// <remarks>
    /// Nothing is printed if the table has no columns.
    /// </remarks>
    public void Print(IReadOnlyList<TElement> elements, Margin leftMargin = default)
    {
        if (columns.Length == 0 || elements.Count == 0)
        {
            return;
        }

        var headerWidth = columns.Max(column => column.Header.Length);
        var margin = new string(' ', leftMargin.Value);

        foreach (var element in elements)
        {
            Console.WriteLine();
            Console.Write(margin);
            Console.WriteLine(columns[0].GetValueAsString(element));

            for (var i = 1; i < columns.Length; ++i)
            {
                var column = columns[i];
                var value = column.GetValueAsString(element);

                Console.Write(margin);
                Console.WriteLine($"  └ {column.Header.PadRight(headerWidth)}: {value}");
            }
        }

        Console.WriteLine();
    }
}
