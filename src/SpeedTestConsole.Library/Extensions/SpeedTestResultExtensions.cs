namespace SpeedTestConsole.Library.Extensions;

public static class SpeedTestResultExtensions
{
    /// <summary>
    /// Calculate the speed
    /// </summary>
    public static string GetSpeedString(this SpeedTestResult result, SpeedUnit unit, SpeedUnitSystem unitSystem)
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
            return $"{bitsPerSecond:0.##} bps";
        }
        else if (bitsPerSecond < 1000000)
        {
            return $"{(bitsPerSecond / 1000):0.##} Kbps";
        }
        else if (bitsPerSecond < 1000000000)
        {
            return $"{(bitsPerSecond / 1000000):0.##} Mbps";
        }
        else if (bitsPerSecond < 1000000000000)
        {
            return $"{(bitsPerSecond / 1000000000):0.##} Gbps";
        }
        else
        {
            return $"{(bitsPerSecond / 1000000000000):0.##} Tbps";
        }
    }

    private static string GetSpeedString_BytesPerSecond(SpeedTestResult result)
    {
        double bytesPerSecond = result.BytesProcessed / ((double)result.ElapsedMilliseconds / 1000);

        if (bytesPerSecond < 1000)
        {
            return $"{bytesPerSecond:0.##} Bps";
        }
        else if (bytesPerSecond < 1000000)
        {
            return $"{(bytesPerSecond / 1000):0.##} KBps";
        }
        else if (bytesPerSecond < 1000000000)
        {
            return $"{(bytesPerSecond / 1000000):0.##} MBps";
        }
        else if (bytesPerSecond < 1000000000000)
        {
            return $"{(bytesPerSecond / 1000000000):0.##} GBps";
        }
        else
        {
            return $"{(bytesPerSecond / 1000000000000):0.##} TBps";
        }
    }
}