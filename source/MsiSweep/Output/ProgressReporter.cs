// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;

namespace MsiSweep.Output;

/// <summary>
/// Creates progress callbacks for moving package files.
/// </summary>
internal static class ProgressReporter
{
    /// <summary>
    /// Creates a callback that writes the progress to the console, overwriting the current line.
    /// Files that could not be moved are reported on the error output.
    /// </summary>
    /// <param name="operation">The past-tense verb shown in the progress line, for example <c>Moved</c>.</param>
    /// <param name="targetDir">The directory the files are moved to.</param>
    /// <returns>The progress callback.</returns>
    public static Action<MoveLocalPackageFilesProgress> CreateConsoleReporter(
        string operation,
        string targetDir) => progress =>
    {
        if (progress.Exception is { } ex)
        {
            Console.Error.WriteLine($"\nSkipped {progress.LocalPackageFile.FileName}: {ex.Message}");
        }
        else
        {
            Console.Write($"\r{operation} {progress.Index + 1}/{progress.TotalCount} packages to \"{targetDir}\"");
        }
    };
}
