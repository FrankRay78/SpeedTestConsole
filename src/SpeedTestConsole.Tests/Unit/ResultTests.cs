using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Tests.Unit;

public class ResultTests
{
    [InlineData(1, 1000, "1 B/s")]
    [InlineData(1000, 1000, "1 KB/s")]
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
