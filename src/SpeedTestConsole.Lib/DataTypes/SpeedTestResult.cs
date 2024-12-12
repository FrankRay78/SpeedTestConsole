namespace SpeedTestConsole.Lib.DataTypes;

public struct SpeedTestResult
{
    public SpeedUnit SpeedUnit { get; set; }
    public double DownloadSpeed { get; set; }
    public double UploadSpeed { get; set; }
    public int Latency { get; set; }

    public SpeedTestResult(SpeedUnit speedUnit, double downloadSpeed, double uploadSpeed, int latency)
    {
        SpeedUnit = speedUnit;
        DownloadSpeed = downloadSpeed;
        UploadSpeed = uploadSpeed;
        Latency = latency;
    }
}