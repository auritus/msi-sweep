// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Model;

namespace MsiSweep.Inventory;

/// <summary>
/// Moves package files to another directory.
/// </summary>
internal static class LocalPackageFileMover
{
    /// <summary>
    /// Moves the package files to <paramref name="targetDirectory"/>. Existing files are not overwritten.
    /// </summary>
    /// <param name="packageFiles">The package files to move.</param>
    /// <param name="targetDirectory">The directory to move the files to. It is created if it does not exist.</param>
    /// <param name="progress">Called after each file, or <see langword="null"/>.</param>
    /// <returns>The number of files that were moved and the number of files that could not be moved.</returns>
    public static MovePackageFilesResult MovePackageFiles(
        IReadOnlyList<LocalPackageFile> packageFiles,
        string targetDirectory,
        Action<MoveLocalPackageFilesProgress>? progress = null)
    {
        Directory.CreateDirectory(targetDirectory);

        var index = 0;
        var failed = 0;

        foreach (var file in packageFiles)
        {
            var targetPath = Path.Join(targetDirectory, file.FileName);
            var exception = default(Exception?);

            try
            {
                File.Move(file.LocalPackagePath.Value, targetPath, overwrite: false);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                failed++;
                exception = ex;
            }

            progress?.Invoke(new MoveLocalPackageFilesProgress(
                file,
                index++,
                packageFiles.Count,
                exception));
        }

        return new MovePackageFilesResult(
            Succeeded: packageFiles.Count - failed,
            Failed: failed);
    }
}
