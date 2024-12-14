namespace SpeedTestConsole.Lib.Client;

public interface ISpeedTestClient
{
    #region Frank

    public Task<Server[]> GetServersAsync();

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