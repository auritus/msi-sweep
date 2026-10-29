// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using MsiSweep.Extensions;

namespace MsiSweep.Tests;

public sealed class GuidExtensionsTests
{
    public readonly static Guid Code = new("12345678-ABCD-EF01-2345-6789ABCDEF01");

    public const string Packed = "87654321DCBA10FE32547698BADCFE10";

    [Fact]
    public void ToPackedString_ReturnsPackedForm()
    {
        Assert.Equal(Packed, Code.ToPackedString());
    }

    [Fact]
    public void TryParsePackedString_ReturnsGuid()
    {
        Assert.True(Guid.TryParsePackedString(Packed, out var result));
        Assert.Equal(Code, result);
    }

    [Fact]
    public void TryParsePackedString_AcceptsLowercase()
    {
        Assert.True(Guid.TryParsePackedString(Packed.ToLowerInvariant(), out var result));
        Assert.Equal(Code, result);
    }

    [Fact]
    public void PackedString_RoundTrips()
    {
        var code = Guid.NewGuid();

        Assert.True(Guid.TryParsePackedString(code.ToPackedString(), out var result));
        Assert.Equal(code, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("87654321DCBA10FE32547698BADCFE1")]
    [InlineData("87654321DCBA10FE32547698BADCFE100")]
    [InlineData("87654321DCBA10FE32547698BADCFEXY")]
    [InlineData("{12345678-ABCD-EF01-2345-6789ABCDEF01}")]
    public void TryParsePackedString_RejectsInvalidInput(string packed)
    {
        Assert.False(Guid.TryParsePackedString(packed, out var result));
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void TryWritePackedString_FailsForShortBuffer()
    {
        Span<char> buffer = stackalloc char[GuidExtensions.PackedLength - 1];

        Assert.False(Code.TryWritePackedString(buffer));
    }

    [Fact]
    public void ToApiString_ReturnsUppercaseBracedForm()
    {
        Assert.Equal("{12345678-ABCD-EF01-2345-6789ABCDEF01}", Code.ToApiString());
    }
}
