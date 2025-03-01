namespace SpeedTestConsole.Library.Client;

/// <summary>
/// Configuration unique to Ookla Speedtest.
/// </summary>
/// <see cref="https://www.speedtest.net/"/>
public sealed class OoklaSpeedtestSettings
{
    public string ServersUrl = "http://www.speedtest.net/speedtest-servers.php";

    // These are used to generate the url for downloading test files.
    // eg: random1500x1500.jpg
    public int[] DownloadSizes = { 1500, 2000, 3000, 3500, 4000 };
    public int DownloadSizeIterations = 4;

    // The default timeout for HttpClient is 100 seconds.
    // ref: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-9.0
    public int DefaultHttpTimeoutMilliseconds = 100000;

    public int LatencyTestIterations = 4;
    public int DownloadParallelTasks = 8;
}