namespace SpeedTestConsole.Lib.Client;

public interface ISpeedTestClient
{
    public Task<Server[]> GetServersAsync();
    public Task<int?> GetServerLatencyAsync(Server server);
    public Task<(Server server, int latency)?> GetFastestServerByLatencyAsync(Server[] servers);
    public Task<(long bytesProcessed, long elapsedMilliseconds)> GetDownloadSpeedAsync(Server server);
}