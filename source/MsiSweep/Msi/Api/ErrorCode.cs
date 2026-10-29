// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Msi.Api;

internal static class ErrorCode
{
    public const uint Success = 0;
    public const uint MoreData = 234;
    public const uint NoMoreItems = 259;

    public const uint UnknownProduct = 1605;
    public const uint UnknownProperty = 1608;
    public const uint DatatypeMismatch = 1629;

    public const uint UnknownPatch = 1647;
}
