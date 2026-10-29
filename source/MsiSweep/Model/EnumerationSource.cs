// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Specifies where a registered package was found.
/// </summary>
public enum EnumerationSource
{
    /// <summary>
    /// The package was found through the Windows Installer API.
    /// </summary>
    Msi,

    /// <summary>
    /// The package was found in the Windows Installer <c>UserData</c> registry key.
    /// </summary>
    Registry
}
