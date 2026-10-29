// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Tests;

/// <summary>
/// A temporary directory that is deleted on dispose.
/// </summary>
internal sealed class TestDirectory : IDisposable
{
    public TestDirectory()
    {
        Path = System.IO.Path.Join(
            System.IO.Path.GetTempPath(),
            "MsiSweep.Tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    /// <summary>
    /// Creates a file of the given size and returns its full path.
    /// </summary>
    public string CreateFile(string name, int size = 1)
    {
        var path = System.IO.Path.Join(Path, name);

        File.WriteAllBytes(path, new byte[size]);
        return path;
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch (IOException)
        {
            // Ignore
        }
    }
}
