namespace SpeedTestConsole.Lib.DataTypes;

public sealed class ProgressInfo
{
    /// <summary>
    /// The numnber of bytes processed since the last progress update.
    /// </summary>
    public long BytesProcessed { get; set; }

    /// <summary>
    ///  The total number of bytes processed since the test started.
    /// </summary>
    public long TotalBytesProcessed { get; set; }

    /// <summary>
    /// The calculated speed.
    /// </summary>
    public double Speed { get; set; }
}