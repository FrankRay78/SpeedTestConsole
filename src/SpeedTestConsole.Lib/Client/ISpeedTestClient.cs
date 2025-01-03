namespace SpeedTestConsole.Lib.Client;

public interface ISpeedTestClient
{
    public Task<Server[]> GetServersAsync();
    public Task<int?> GetServerLatencyAsync(Server server);
    public Task<(Server server, int latency)?> GetFastestServerByLatencyAsync(Server[] servers);

    /// <summary>
    /// Measures the download speed of the specified server.
    /// </summary>
    /// <param name="server">The server to measure download speed from.</param>
    /// <returns>A tuple containing bytes processed and elapsed time in milliseconds.</returns>
    public Task<(long bytesProcessed, long elapsedMilliseconds)> GetDownloadSpeedAsync(Server server);

    /// <summary>
    /// Measures the download speed of the specified server.
    /// </summary>
    /// <param name="server">The server to measure download speed from.</param>
    /// <param name="UpdateProgress">An action that receives the download progress percentage (0 to 100).</param>
    /// <returns>A tuple containing bytes processed and elapsed time in milliseconds.</returns>
    public Task<(long bytesProcessed, long elapsedMilliseconds)> GetDownloadSpeedAsync(Server server, Action<int> UpdateProgress);
}