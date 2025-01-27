namespace SpeedTestConsole.Lib.Extensions;

public static class ResultExtensions
{
    public static string GetSpeedString(this (long bytesProcessed, long elapsedMilliseconds) result)
    {
        // Calculate the download speed
        var bytesPerSecond = ByteSize.FromBytes(result.bytesProcessed / ((double)result.elapsedMilliseconds / 1000));

        return $"{bytesPerSecond.ToString()}/s";
    }
}