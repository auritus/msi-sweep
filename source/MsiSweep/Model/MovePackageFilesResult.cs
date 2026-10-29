// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Reports the outcome of an operation on several package files.
/// </summary>
/// <param name="Succeeded">The number of files that were processed successfully.</param>
/// <param name="Failed">The number of files that could not be processed.</param>
public readonly record struct MovePackageFilesResult(
    int Succeeded,
    int Failed);
