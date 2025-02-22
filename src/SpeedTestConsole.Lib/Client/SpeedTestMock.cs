namespace SpeedTestConsole.Lib.Client;

/// <summary>
/// A mock implementation of <see cref="ISpeedTestClient"/> for testing purposes.
/// </summary>
public class SpeedTestMock : ISpeedTestClient
{
    // Delegates for method behavior
    public Func<Task<IServer[]>>? GetServersAsyncFunc { get; set; }
    public Func<IServer, Task<int?>>? GetServerLatencyAsyncFunc { get; set; }
    public Func<IServer[], Task<(IServer server, int latency)?>>? GetFastestServerByLatencyAsyncFunc { get; set; }
    public Func<IServer, Task<SpeedTestResult>>? GetDownloadSpeedAsyncFunc { get; set; }
    public Func<IServer, Action<int>, Task<SpeedTestResult>>? GetDownloadSpeedWithProgressAsyncFunc { get; set; }

    public Task<IServer[]> GetServersAsync()
    {
        if (GetServersAsyncFunc != null)
            return GetServersAsyncFunc();
        throw new NotImplementedException();
    }

    public Task<int?> GetServerLatencyAsync(IServer server)
    {
        if (GetServerLatencyAsyncFunc != null)
            return GetServerLatencyAsyncFunc(server);
        throw new NotImplementedException();
    }

    public Task<(IServer server, int latency)?> GetFastestServerByLatencyAsync(IServer[] servers)
    {
        if (GetFastestServerByLatencyAsyncFunc != null)
            return GetFastestServerByLatencyAsyncFunc(servers);
        throw new NotImplementedException();
    }

    public Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server)
    {
        if (GetDownloadSpeedAsyncFunc != null)
            return GetDownloadSpeedAsyncFunc(server);
        throw new NotImplementedException();
    }

    public Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server, Action<int> updateProgress)
    {
        if (GetDownloadSpeedWithProgressAsyncFunc != null)
            return GetDownloadSpeedWithProgressAsyncFunc(server, updateProgress);
        throw new NotImplementedException();
    }
}
