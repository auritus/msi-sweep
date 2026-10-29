// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Text;

namespace MsiSweep.Output;

/// <summary>
/// Prints elements to the console as a table with one row per element.
/// </summary>
/// <typeparam name="TElement">The type of the elements shown in the table.</typeparam>
/// <param name="columns">The columns of the table.</param>
public sealed class Table<TElement>(IColumn<TElement>[] columns)
{
    /// <summary>
    /// Prints the header row and one row for each element.
    /// </summary>
    /// <param name="elements">The elements to print. The sequence is enumerated twice.</param>
    /// <param name="leftMargin">The number of columns to indent the first column by.</param>
    /// <remarks>
    /// Each column is as wide as its widest cell. Rows are printed in alternating colors.
    /// </remarks>
    public void Print(
        IReadOnlyList<TElement> elements,
        Margin leftMargin = default)
    {
        if (elements.Count == 0)
        {
            return;
        }

        Span<ColumnContext> contexts = stackalloc ColumnContext[columns.Length];
        CreateColumnContext(contexts, elements, leftMargin);

        var separator = new string('-', count: GetSeparatorWidth(contexts));
        var line = new StringBuilder();

        Console.WriteLine();
        Console.WriteLine(separator);

        for (var i = 0; i < columns.Length; ++i)
        {
            ref var column = ref columns[i];
            ref var context = ref contexts[i];

            AppendCell(line, column.Header, context with { Alignment = ColumnAlignment.Left });
        }

        WriteLine(line);

        var next = ConsoleColor.Gray;

        Console.WriteLine(separator);
        Console.ForegroundColor = ConsoleColor.DarkGray;

        foreach (var element in elements)
        {
            for (var i = 0; i < columns.Length; ++i)
            {
                ref var column = ref columns[i];
                ref var context = ref contexts[i];

                AppendCell(line, column.GetValueAsString(element), context);
            }

            WriteLine(line);

            (next, Console.ForegroundColor) = (Console.ForegroundColor, next);
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    #region Implementation
    private const int ColumnGap = 2;

    private static void AppendCell(StringBuilder line, string value, in ColumnContext context)
    {
        var padding = Math.Max(0, context.Width - value.Length);

        line.Append(' ', context.Margin);

        if (context.Alignment == ColumnAlignment.Right)
        {
            line.Append(' ', padding).Append(value);
        }
        else
        {
            line.Append(value).Append(' ', padding);
        }

        line.Append(' ', ColumnGap);
    }

    private static void WriteLine(StringBuilder line)
    {
        Console.WriteLine(line.ToString().TrimEnd());
        line.Clear();
    }

    private static int GetSeparatorWidth(ReadOnlySpan<ColumnContext> contexts)
    {
        if (Console.IsOutputRedirected is false)
        {
            return Math.Max(0, Console.WindowWidth - 1);
        }

        var width = -ColumnGap;

        foreach (var context in contexts)
        {
            width += context.Margin + context.Width + ColumnGap;
        }

        return Math.Max(0, width);
    }

    private void CreateColumnContext(
        Span<ColumnContext> contexts,
        IReadOnlyList<TElement> elements,
        Margin margin)
    {
        Span<int> widths = stackalloc int[columns.Length];

        for (var i = 0; i < columns.Length; ++i)
        {
            widths[i] = columns[i].Header.Length;
        }

        foreach (var element in elements)
        {
            for (var i = 0; i < columns.Length; ++i)
            {
                widths[i] = Math.Max(widths[i], columns[i].GetValueAsString(element).Length);
            }
        }

        for (var i = 0; i < columns.Length; ++i)
        {
            contexts[i] = new ColumnContext(
                Width: widths[i],
                Margin: i == 0 ? margin.Value : default,
                Alignment: columns[i].Alignment);
        }
    }

    private readonly record struct ColumnContext(
        int Width,
        int Margin,
        ColumnAlignment Alignment);
    #endregion
}
