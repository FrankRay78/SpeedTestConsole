namespace SpeedTestConsole.Lib;

public sealed class Constants
{
    public const string ServersUrl = "http://www.speedtest.net/speedtest-servers.php";
    public const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const int MaxUploadSize = 6;
    public static readonly int[] DownloadSizes = { 1500, 2000, 3000, 3500, 4000 };
}