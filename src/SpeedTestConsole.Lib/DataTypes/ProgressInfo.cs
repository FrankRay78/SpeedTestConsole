namespace SpeedTestConsole.Lib.DataTypes;

public sealed class ProgressInfo
{
    public long TotalBytes { get; set; }
    public long BytesProcessed { get; set; }
    public double Speed { get; set; }
}