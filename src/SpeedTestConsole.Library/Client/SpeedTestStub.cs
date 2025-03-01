namespace SpeedTestConsole.Library.Client;

/// <summary>
/// A stub implementation of <see cref="ISpeedTestService"/> for testing purposes.
/// </summary>
public class SpeedTestStub : ISpeedTestService
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

    public Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server)
    {
        return GetDownloadSpeedAsync(server, _ => { });
    }

    public Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server, Action<int> updateProgress)
    {
        if (updateProgress is not null)
        {
            updateProgress(25);
            updateProgress(50);
            updateProgress(75);
            updateProgress(100);
        }

        return Task.FromResult<SpeedTestResult>(new SpeedTestResult() { BytesProcessed = 1000, ElapsedMilliseconds = 1000 });
    }

    public Task<SpeedTestResult> GetUploadSpeedAsync(IServer server)
    {
        return GetUploadSpeedAsync(server, (int _) => { });
    }

    public Task<SpeedTestResult> GetUploadSpeedAsync(IServer server, Action<int> updateProgress)
    {
        if (updateProgress is not null)
        {
            updateProgress(25);
            updateProgress(50);
            updateProgress(75);
            updateProgress(100);
        }

        return Task.FromResult<SpeedTestResult>(new SpeedTestResult() { BytesProcessed = 1000, ElapsedMilliseconds = 1000 });
    }
}
