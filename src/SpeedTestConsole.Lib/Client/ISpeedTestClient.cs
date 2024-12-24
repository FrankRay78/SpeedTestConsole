namespace SpeedTestConsole.Lib.Client;

public interface ISpeedTestClient
{
    #region Frank

    public Task<Server[]> GetServersAsync();
    public Task GetServerLatencyAsync(Server[] servers, bool useServerLatencyToReduceHttpClientTimeout);
    public Task GetServerLatencyAsync(Server server);
    public Task GetServerLatencyAsync(Server server, int httpTimeoutMilliseconds);
    public Task<SpeedTestResult> GetDownloadSpeedAsync(Server server);

    #endregion

    public TestStage CurrentStage { get; }
    public SpeedUnit SpeedUnit { get; }
    public event EventHandler<TestStage>? StageChanged;
    public event EventHandler<ProgressInfo>? ProgressChanged;
    public Task<SpeedTestResult> TestSpeedAsync(SpeedUnit speedUnit,
        int parallelTasks = 8,
        bool testLatency = true,
        bool testDownload = true,
        bool testUpload = true);
}