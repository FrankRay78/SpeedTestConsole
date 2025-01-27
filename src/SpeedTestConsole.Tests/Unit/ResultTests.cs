using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Tests.Unit;

public class ResultTests
{
    [InlineData(1, 1000, "1 B/s")]
    [InlineData(999, 1000, "999 B/s")]
    [InlineData(1000, 1000, "1 KB/s")]
    [InlineData(10000, 1000, "10 KB/s")]
    [InlineData(100000, 1000, "100 KB/s")]
    [InlineData(900000, 1000, "900 KB/s")]
    [InlineData(990000, 1000, "990 KB/s")]
    [InlineData(999000, 1000, "999 KB/s")]
    [InlineData(999900, 1000, "999.9 KB/s")]
    [InlineData(999990, 1000, "999.99 KB/s")]
    [InlineData(999994, 1000, "999.99 KB/s")]
    //
    // NB. ByteSize.ToString() recognises KB but rounds up to two decimal places
    // ie. Resulting in 1000, rather than outputing 1 MB (as per the test two down)
    [InlineData(999995, 1000, "1000 KB/s")]
    [InlineData(999999, 1000, "1000 KB/s")]
    [InlineData(1000000, 1000, "1 MB/s")]
    [InlineData(1000000000, 1000, "1 GB/s")]
    [InlineData(1000000000000, 1000, "1 TB/s")]
    [Theory]
    public void Should_Calculate_Bytes_Per_Second_Correctly(long bytesProcessed, long elapsedMilliseconds, string expected)
    {
        // Given
        var result = (bytesProcessed, elapsedMilliseconds);

        // When
        var speedString = result.GetSpeedString();

        // Then
        Assert.Equal($"{expected}", speedString);
    }
}
