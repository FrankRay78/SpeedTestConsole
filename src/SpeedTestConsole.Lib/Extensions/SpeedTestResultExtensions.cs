namespace SpeedTestConsole.Lib.Extensions;

public static class SpeedTestResultExtensions
{
    ///// <summary>
    ///// Calculate the speed
    ///// </summary>
    //public static string GetSpeedString(this SpeedTestResult result, SpeedUnit unit)
    //{
    //    string speedString = string.Empty;

    //    if (unit == SpeedUnit.BitsPerSecond)
    //    {
    //        var bitsPerSecond = (result.BytesProcessed * 8) / ((double)result.ElapsedMilliseconds / 1000);

    //        if (bitsPerSecond < 1000)
    //        {
    //            speedString = $"{bitsPerSecond.ToString()} b/s";
    //        }
    //        else if (bitsPerSecond < 1000000)
    //        {
    //            speedString = $"{(bitsPerSecond / 1000).ToString()} Kb/s";
    //        }
    //        else if (bitsPerSecond < 1000000000)
    //        {
    //            speedString = $"{(bitsPerSecond / 1000000).ToString()} Mb/s";
    //        }
    //        else if (bitsPerSecond < 1000000000000)
    //        {
    //            speedString = $"{(bitsPerSecond / 1000000000).ToString()} Gb/s";
    //        }
    //        else
    //        {
    //            speedString = $"{(bitsPerSecond / 1000000000000).ToString()} Tb/s";
    //        }
    //    }
    //    else if (unit == SpeedUnit.BytesPerSecond)
    //    {
    //        var bytesPerSecond = ByteSize.FromBytes(result.BytesProcessed / ((double)result.ElapsedMilliseconds / 1000));

    //        // Avoid ByteSize reverting to bits
    //        if (bytesPerSecond.Bytes < 1)
    //        {
    //            speedString = $"{bytesPerSecond.ToString("0.## B")}/s";
    //        }
    //        else
    //        {
    //            speedString = $"{bytesPerSecond.ToString()}/s";
    //        }
    //    }
    //    else
    //    {
    //        throw new Exception($"Unknown speed unit: {nameof(unit)}");
    //    }

    //    return speedString;
    //}

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
            return $"{bitsPerSecond.ToString("0.##")} b/s";
        }
        else if (bitsPerSecond < 1000000)
        {
            return $"{(bitsPerSecond / 1000).ToString("0.##")} Kb/s";
        }
        else if (bitsPerSecond < 1000000000)
        {
            return $"{(bitsPerSecond / 1000000).ToString("0.##")} Mb/s";
        }
        else if (bitsPerSecond < 1000000000000)
        {
            return $"{(bitsPerSecond / 1000000000).ToString("0.##")} Gb/s";
        }
        else
        {
            return $"{(bitsPerSecond / 1000000000000).ToString("0.##")} Tb/s";
        }
    }

    private static string GetSpeedString_BytesPerSecond(SpeedTestResult result)
    {
        var bytesPerSecond = ByteSize.FromBytes(result.BytesProcessed / ((double)result.ElapsedMilliseconds / 1000));

        // Avoid ByteSize reverting to bits
        if (bytesPerSecond.Bytes < 1)
        {
            return $"{bytesPerSecond.ToString("0.## B")}/s";
        }
        else
        {
            return $"{bytesPerSecond.ToString("0.##")}/s";
        }
    }
}