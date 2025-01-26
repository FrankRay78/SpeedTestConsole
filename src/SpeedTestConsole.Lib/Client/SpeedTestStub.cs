namespace SpeedTestConsole.Lib.Client;

/// <summary>
/// A stub implementation of <see cref="ISpeedTestClient"/> for testing purposes.
/// </summary>
public class SpeedTestStub : ISpeedTestClient
{
    public Task<IServer[]> GetServersAsync()
    {
        return Task.FromResult(new IServer[]
        {
            new Server { Name = "Test Server 1", Sponsor = "Test Sponsor 1", Url = "http://test1.com" },
            new Server { Name = "Test Server 2", Sponsor = "Test Sponsor 2", Url = "http://test2.com" },
            new Server { Name = "Test Server 3", Sponsor = "Test Sponsor 3", Url = "http://test3.com" },
        });
    }

    public Task<int?> GetServerLatencyAsync(IServer server)
    {
        var latency = int.Parse(server.Name!.Replace("Test Server", "")) * 100;

        return Task.FromResult<int?>(latency);
    }

    public Task<(IServer server, int latency)?> GetFastestServerByLatencyAsync(IServer[] servers)
    {
        var fastestServer = servers[0];
        var fastestLatency = int.Parse(fastestServer.Name!.Replace("Test Server", "")) * 100;

        return Task.FromResult<(IServer server, int latency)?>((fastestServer, fastestLatency));
    }

    public Task<(long bytesProcessed, long elapsedMilliseconds)> GetDownloadSpeedAsync(IServer server)
    {
        return GetDownloadSpeedAsync(server, _ => { });
    }

    public Task<(long bytesProcessed, long elapsedMilliseconds)> GetDownloadSpeedAsync(IServer server, Action<int> updateProgress)
    {
        if (updateProgress is not null)
        {
            updateProgress(25);
            updateProgress(50);
            updateProgress(75);
            updateProgress(100);
        }

        return Task.FromResult<(long bytesProcessed, long elapsedMilliseconds)>((1000, 1000));
    }
}
