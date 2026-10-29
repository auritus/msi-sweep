// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Msi.Api;

[Flags]
internal enum InstallContext : uint
{
    UserManaged = 1,
    UserUnmanaged = 2,
    Machine = 4,

    All = UserManaged | UserUnmanaged | Machine
}
