// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

namespace MsiSweep.Msi.Api;

/// <summary>
/// Representation of a <c>marshalable</c> Security Identifier (SID) with a capacity of 256 characters.
/// This structure is designed for interoperability with unmanaged code, allowing for the safe handling of SIDs in a managed environment.
/// </summary>
/// <remarks>
/// Note that the <see cref="Length"/> field is initialized with the buffer capacity, since it is usually
/// use as [IN, OUT] parameter in native calls, indicating the capacity of the buffer before the call and
/// the actual length of the SID after the call.
///
/// Consider recreating a new instance to avoid <see cref="ErrorCode.MoreData"/> or make sure to reset the
/// <see cref="Length"/> field to <see cref="InlineSid.Capacity"/> before passing it to a native call again.
/// </remarks>
internal struct Sid()
{
    public InlineSid Buffer;

    public uint Length = InlineSid.Capacity;

    #region Overrides
    public override string? ToString() => Buffer.ToString(Length);
    #endregion
}
