// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Reports the result of a single move operation.
/// </summary>
/// <param name="LocalPackageFile">The package file that was moved.</param>
/// <param name="Index">The zero-based index of the file in the files to move.</param>
/// <param name="TotalCount">The total number of files to move.</param>
/// <param name="Exception">
/// The exception that prevented the move, or <see langword="null"/> if the file was moved.
/// </param>
public readonly record struct MoveLocalPackageFilesProgress(
    LocalPackageFile LocalPackageFile,
    int Index,
    int TotalCount,
    Exception? Exception);
