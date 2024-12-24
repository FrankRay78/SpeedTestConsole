﻿namespace SpeedTestConsole.Lib;

public sealed class Settings
{
    //Constants
    public string ServersUrl = "http://www.speedtest.net/speedtest-servers.php";
    public string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public int MaxUploadSize = 6;
    public readonly int[] DownloadSizes = { 1500, 2000, 3000, 3500, 4000 };

    //GetServerLatencyAsync
    public int ServerLatencyIterations = 4;

    //GetDownloadSpeedAsync
    public SpeedUnit SpeedUnit = SpeedUnit.Mbps;
    public int DownloadParallelTasks = 8;
}