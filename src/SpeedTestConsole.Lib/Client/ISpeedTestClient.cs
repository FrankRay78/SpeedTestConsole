namespace SpeedTestConsole.Lib.Client;

public interface ISpeedTestClient
{
    public event EventHandler<ProgressInfo>? ProgressChanged;

    public Task<Server[]> GetServersAsync();
    public Task<int?> GetServerLatencyAsync(Server server);
    public Task<(Server server, int latency)?> GetFastestServerByLatencyAsync(Server[] servers);

    public Task<SpeedTestResult> GetDownloadSpeedAsync(Server server);
}