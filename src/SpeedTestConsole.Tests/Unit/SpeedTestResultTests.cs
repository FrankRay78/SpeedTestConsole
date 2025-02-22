using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Tests.Unit;

public class SpeedTestResultTests
{
    [InlineData(1, 1000, "8 b/s")]
    [InlineData(2, 1000, "16 b/s")]
    [InlineData(124, 1000, "992 b/s")]
    [InlineData(125, 1000, "1 Kb/s")]
    [InlineData(125, 2000, "500 b/s")]
    [InlineData(125, 4000, "250 b/s")]
    [InlineData(125, 4500, "222.22 b/s")]
    [InlineData(256, 1000, "2.05 Kb/s")]
    [InlineData(125000, 1000, "1 Mb/s")]
    [InlineData(125000000, 1000, "1 Gb/s")]
    [InlineData(250000000, 1000, "2 Gb/s")]
    [InlineData(125000000000, 1000, "1 Tb/s")]
    [InlineData(250000000000, 1000, "2 Tb/s")]
    //Adjust the milliseconds to test fractional rounding
    [InlineData(1, 2000, "4 b/s")]
    [InlineData(1, 4000, "2 b/s")]
    [InlineData(1, 8000, "1 b/s")]
    [InlineData(1, 16000, "0.5 b/s")]
    [InlineData(1, 32000, "0.25 b/s")]
    [InlineData(1, 64000, "0.13 b/s")]
    [Theory]
    public void Should_Calculate_Bits_Per_Second_Correctly(long bytesProcessed, long elapsedMilliseconds, string expected)
    {
        // Given
        var result = new SpeedTestResult { BytesProcessed = bytesProcessed, ElapsedMilliseconds = elapsedMilliseconds };

        // When
        var speedString = result.GetSpeedString(SpeedUnit.BitsPerSecond);

        // Then
        Assert.Equal($"{expected}", speedString);
    }

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
    //Adjust the milliseconds to test fractional rounding
    [InlineData(1, 2000, "0.5 B/s")]
    [InlineData(1, 4000, "0.25 B/s")]
    [InlineData(1, 8000, "0.13 B/s")]
    [Theory]
    public void Should_Calculate_Bytes_Per_Second_Correctly(long bytesProcessed, long elapsedMilliseconds, string expected)
    {
        // Given
        var result = new SpeedTestResult { BytesProcessed = bytesProcessed, ElapsedMilliseconds = elapsedMilliseconds };

        // When
        var speedString = result.GetSpeedString(SpeedUnit.BytesPerSecond);

        // Then
        Assert.Equal($"{expected}", speedString);
    }
}
