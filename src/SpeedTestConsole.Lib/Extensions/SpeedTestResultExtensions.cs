namespace SpeedTestConsole.Lib.Extensions;

public static class SpeedTestResultExtensions
{
    /// <summary>
    /// Calculate the speed
    /// </summary>
    public static string GetSpeedString(this SpeedTestResult result, SpeedUnit unit)
    {
        if (unit == SpeedUnit.BitsPerSecond)
        {
            return GetSpeedString_BitsPerSecond(result);
        }
        else if (unit == SpeedUnit.BytesPerSecond)
        {
            return GetSpeedString_BytesPerSecond(result);
        }
        else
        {
            throw new Exception($"Unknown speed unit: {nameof(unit)}");
        }
    }

    private static string GetSpeedString_BitsPerSecond(SpeedTestResult result)
    {
        var bitsPerSecond = (result.BytesProcessed * 8) / ((double)result.ElapsedMilliseconds / 1000);

        if (bitsPerSecond < 1000)
        {
            return $"{bitsPerSecond.ToString("0.##")} bps";
        }
        else if (bitsPerSecond < 1000000)
        {
            return $"{(bitsPerSecond / 1000).ToString("0.##")} Kbps";
        }
        else if (bitsPerSecond < 1000000000)
        {
            return $"{(bitsPerSecond / 1000000).ToString("0.##")} Mbps";
        }
        else if (bitsPerSecond < 1000000000000)
        {
            return $"{(bitsPerSecond / 1000000000).ToString("0.##")} Gbps";
        }
        else
        {
            return $"{(bitsPerSecond / 1000000000000).ToString("0.##")} Tbps";
        }
    }

    private static string GetSpeedString_BytesPerSecond(SpeedTestResult result)
    {
        var bytesPerSecond = ByteSize.FromBytes(result.BytesProcessed / ((double)result.ElapsedMilliseconds / 1000));

        // Avoid ByteSize reverting to bits
        if (bytesPerSecond.Bytes < 1)
        {
            return $"{bytesPerSecond.ToString("0.## B")}ps";
        }
        else
        {
            return $"{bytesPerSecond.ToString("0.##")}ps";
        }
    }
}