namespace SpeedTestConsole.Lib.Extensions;

public static class SpeedTestResultExtensions
{
    public static string GetSpeedString(this SpeedTestResult result)
    {
        // Calculate the download speed
        var bytesPerSecond = ByteSize.FromBytes(result.BytesProcessed / ((double)result.ElapsedMilliseconds / 1000));

        return $"{bytesPerSecond.ToString()}/s";
    }
}