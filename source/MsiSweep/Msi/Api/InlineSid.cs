// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsiSweep.Msi.Api;

[InlineArray(Capacity)]
internal struct InlineSid
{
    public const int Capacity = 256;

    public string? ToString(uint length)
    {
        return length == 0
            ? null
            : new string(MemoryMarshal.CreateSpan(ref _buffer, (int)length));
    }

    #region Implementation
    private char _buffer;
    #endregion
}
