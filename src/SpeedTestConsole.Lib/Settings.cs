namespace SpeedTestConsole.Lib;

public sealed class Settings
{
    // Constants
    public string ServersUrl = "http://www.speedtest.net/speedtest-servers.php";
    public string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public int MaxUploadSize = 6;
    public readonly int[] DownloadSizes = { 1500, 2000, 3000, 3500, 4000 };

    // GetServerLatencyAsync

    // The default timeout for HttpClient is 100 seconds.
    // ref: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-9.0
    public int DefaultHttpTimeoutMilliseconds = 100000;

    public int ServerLatencyIterations = 4;

    // GetDownloadSpeedAsync
    public SpeedUnit SpeedUnit = SpeedUnit.Mbps;
    public int DownloadParallelTasks = 8;
}