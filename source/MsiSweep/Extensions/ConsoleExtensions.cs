// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Extensions;

/// <summary>
/// Extension methods for the <see cref="Console"/> class.
/// </summary>
internal static class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// Writes the given <paramref name="title"/> and underlines it with <c>-</c> characters.
        /// </summary>
        /// <param name="title">The title to print.</param>
        public static void WriteHeader(string title)
        {
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length));
        }
    }
}
