// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsiSweep.Msi.Api;

[InlineArray(ZeroTerminatedLength)]
internal struct InlineGuid()
{
    public Guid ToGuid() => Guid.Parse(AsReadOnlySpan());

    #region Implementation
    private const int ZeroTerminatedLength = 39;

    private char _buffer;

    private ReadOnlySpan<char> AsReadOnlySpan()
    {
        return MemoryMarshal.CreateSpan(ref _buffer, ZeroTerminatedLength - 1);
    }
    #endregion
}
