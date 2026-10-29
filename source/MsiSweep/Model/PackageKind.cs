// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Model;

/// <summary>
/// Specifies the kind of a Windows Installer package.
/// </summary>
public enum PackageKind
{
    /// <summary>
    /// A product, installed from an <c>.msi</c> package.
    /// </summary>
    Product,

    /// <summary>
    /// A patch, installed from an <c>.msp</c> package.
    /// </summary>
    Patch
}
