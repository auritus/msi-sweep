// --------------------------------------------------------------------------------
// MsiSweep
//
// MIT License (see LICENSE file for details)
// --------------------------------------------------------------------------------

using System.Globalization;
using MsiSweep.Model;

namespace MsiSweep.Tests;

public sealed class FileSizeTests
{
    public const long KB = 1024;

    public const long TB = KB * KB * KB * KB;

    [Theory]
    [InlineData(0, "0.00 B")]
    [InlineData(1, "1.00 B")]
    [InlineData(1023, "1023.00 B")]
    [InlineData(KB, "1.00 KB")]
    [InlineData(KB + KB / 2, "1.50 KB")]
    [InlineData(KB * KB, "1.00 MB")]
    [InlineData(KB * KB * KB, "1.00 GB")]
    [InlineData(TB, "1.00 TB")]
    [InlineData(TB * KB, "1024.00 TB")]
    public void ToFormatString_UsesLargestUnit(long value, string expected)
    {
        // ToFormatString uses the current culture; CurrentCulture is local to the test's execution context.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Assert.Equal(expected, new FileSize(value).ToHumanizedString());
    }

    [Fact]
    public void ToFormatString_UsesCurrentCulture()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

        Assert.Equal("1,50 KB", new FileSize(KB + KB / 2).ToHumanizedString());
    }

    [Fact]
    public void ToString_ReturnsBytes()
    {
        Assert.Equal("1536", new FileSize(1536).ToString());
    }

    [Fact]
    public void Comparison_UsesValue()
    {
        FileSize small = 1;
        FileSize large = 2;

        Assert.True(small < large);
        Assert.True(large > small);
        Assert.True(small.CompareTo(large) < 0);
        Assert.Equal(new FileSize(1), small);
    }
}
