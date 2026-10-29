// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Msi.Api;

[Flags]
internal enum PatchState : uint
{
    Applied = 1,
    Superseded = 2,
    Obsoleted = 4,
    Registered = 8,

    All = Applied | Superseded | Obsoleted | Registered
}
