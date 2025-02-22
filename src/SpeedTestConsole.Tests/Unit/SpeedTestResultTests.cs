using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Tests.Unit;

public class SpeedTestResultTests
{
    [InlineData(1, 1000, "8 bps")]
    [InlineData(2, 1000, "16 bps")]
    [InlineData(124, 1000, "992 bps")]
    [InlineData(125, 1000, "1 Kbps")]
    [InlineData(125, 2000, "500 bps")]
    [InlineData(125, 4000, "250 bps")]
    [InlineData(125, 4500, "222.22 bps")]
    [InlineData(256, 1000, "2.05 Kbps")]
    [InlineData(125000, 1000, "1 Mbps")]
    [InlineData(125000000, 1000, "1 Gbps")]
    [InlineData(250000000, 1000, "2 Gbps")]
    [InlineData(125000000000, 1000, "1 Tbps")]
    [InlineData(250000000000, 1000, "2 Tbps")]
    //Adjust the milliseconds to test fractional rounding
    [InlineData(1, 2000, "4 bps")]
    [InlineData(1, 4000, "2 bps")]
    [InlineData(1, 8000, "1 bps")]
    [InlineData(1, 16000, "0.5 bps")]
    [InlineData(1, 32000, "0.25 bps")]
    [InlineData(1, 64000, "0.13 bps")]
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

    [InlineData(1, 1000, "1 Bps")]
    [InlineData(999, 1000, "999 Bps")]
    [InlineData(1000, 1000, "1 KBps")]
    [InlineData(10000, 1000, "10 KBps")]
    [InlineData(100000, 1000, "100 KBps")]
    [InlineData(900000, 1000, "900 KBps")]
    [InlineData(990000, 1000, "990 KBps")]
    [InlineData(999000, 1000, "999 KBps")]
    [InlineData(999900, 1000, "999.9 KBps")]
    [InlineData(999990, 1000, "999.99 KBps")]
    [InlineData(999994, 1000, "999.99 KBps")]
    //
    // NB. ByteSize.ToString() recognises KB but rounds up to two decimal places
    // ie. Resulting in 1000, rather than outputing 1 MB (as per the test two down)
    [InlineData(999995, 1000, "1000 KBps")]
    [InlineData(999999, 1000, "1000 KBps")]
    [InlineData(1000000, 1000, "1 MBps")]
    [InlineData(1000000000, 1000, "1 GBps")]
    [InlineData(1000000000000, 1000, "1 TBps")]
    //Adjust the milliseconds to test fractional rounding
    [InlineData(1, 2000, "0.5 Bps")]
    [InlineData(1, 4000, "0.25 Bps")]
    [InlineData(1, 8000, "0.13 Bps")]
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
