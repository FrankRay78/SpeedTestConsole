using System.Diagnostics;
using System.Net.Http.Headers;

namespace SpeedTestConsole.Lib.Client;

/// <summary>
/// An Ookla Speedtest implementation of the <see cref="ISpeedTestClient"/> interface.
/// </summary>
public sealed class OoklaSpeedtest : ISpeedTestClient
{
    private OoklaSpeedtestSettings settings { get; set; }

    public OoklaSpeedtest(OoklaSpeedtestSettings settings)
    {
        this.settings = settings;
    }

    public async Task<IServer[]> GetServersAsync()
    {
        using var httpClient = GetHttpClient();
        var serversXml = await httpClient.GetStringAsync(settings.ServersUrl);
        return serversXml.DeserializeFromXml<ServersList>()?.Servers ?? Array.Empty<Server>();
    }

    public async Task<int?> GetServerLatencyAsync(IServer server)
    {
        return await GetServerLatencyAsync(server, settings.DefaultHttpTimeoutMilliseconds, settings.LatencyTestIterations);
    }

    private async Task<int?> GetServerLatencyAsync(IServer server, int httpTimeoutMilliseconds, int testIterations)
    {
        int? latency = null;

        try
        {
            if (string.IsNullOrWhiteSpace(server.Url))
            {
                throw new NullReferenceException("Server url was null");
            }

            var latencyUrl = GetBaseUrl(server.Url).Append("latency.txt");
            var stopwatch = new Stopwatch();

            using var httpClient = GetHttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(httpTimeoutMilliseconds);


            var iteration = 1;
            do
            {
                stopwatch.Start();
                var testString = await httpClient.GetStringAsync(latencyUrl);
                stopwatch.Stop();

                if (!testString.StartsWith("test=test"))
                {
                    throw new InvalidOperationException("Server returned incorrect test string for latency.txt");
                }

                iteration++;
            }
            while (iteration < testIterations);

            // Calculate the average server latency
            latency = (int)stopwatch.ElapsedMilliseconds / testIterations;
        }
        catch
        {
            // Ignore this server
        }

        return latency;
    }

    public async Task<(IServer server, int latency)?> GetFastestServerByLatencyAsync(IServer[] servers)
    {
        int fastestLatency = settings.DefaultHttpTimeoutMilliseconds;
        IServer? fastestServer = null;

        foreach (var server in servers)
        {
            // nb. Bump up the fastest latency/timeout by a slight margin
            var httpTimeoutMilliseconds = (fastestLatency == settings.DefaultHttpTimeoutMilliseconds ? fastestLatency : (int)(fastestLatency * 1.5));

            var latency = await GetServerLatencyAsync(server, httpTimeoutMilliseconds, settings.LatencyTestIterations);

            if (latency != null && latency < fastestLatency)
            {
                // Reduce the http timeout to the new fastest latency
                // (ie. do not wait for servers that are slower)
                fastestLatency = latency.Value;
                fastestServer = server;
            }
        }

        return (fastestServer == null ? null : (fastestServer, fastestLatency));
    }

    public async Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server)
    {
        return await GetDownloadSpeedAsync(server, (int _) => { });
    }

    public async Task<SpeedTestResult> GetDownloadSpeedAsync(IServer server, Action<int> UpdateProgress)
    {
        if (string.IsNullOrWhiteSpace(server.Url))
        {
            throw new NullReferenceException("Server url was null");
        }

        var downloadUrls = GenerateDownloadUrls(server.Url);

        // Download content from a specified URL and return the size of the data in bytes.
        Func<HttpClient, string, Task<int>> DownloadAndMeasureAsync = async (client, url) =>
        {
            var data = await client.GetStringAsync(url).ConfigureAwait(false);
            return data.Length;
        };

        var downloadResult = await GenericTestSpeedAsync(downloadUrls, DownloadAndMeasureAsync, UpdateProgress, settings.DownloadParallelTasks);

        return downloadResult;
    }

    private async Task<SpeedTestResult> GenericTestSpeedAsync<T>(IEnumerable<T> testData,
        Func<HttpClient, T, Task<int>> doWork,
        Action<int> UpdateProgress,
        int parallelTasks)
    {
        object lockObject = new();
        int completedCount = 0;
        int totalCount = testData.Count();

        var timer = new Stopwatch();
        var throttler = new SemaphoreSlim(parallelTasks);

        timer.Start();

        // Create a list of tasks that will download the test data.
        var downloadTasks = testData.Select(async data =>
        {
            // Each task must acquire a "permit" before it can start executing.
            await throttler.WaitAsync().ConfigureAwait(false);

            using var httpClient = GetHttpClient();
            try
            {
                var size = await doWork(httpClient, data).ConfigureAwait(false);

                lock (lockObject)
                {
                    completedCount++;

                    var percentageComplete = (int)(((double)completedCount / totalCount) * 100);

                    UpdateProgress(percentageComplete);
                }

                return size;
            }
            finally
            {
                // Release the permit so other waiting tasks can proceed.
                throttler.Release();
            }
        }).ToArray();

        await Task.WhenAll(downloadTasks);
        timer.Stop();

        long totalBytesProcessed = downloadTasks.Sum(task => task.Result);
        return new SpeedTestResult() { BytesProcessed = totalBytesProcessed, ElapsedMilliseconds = timer.ElapsedMilliseconds };
    }

    #region Helper Functions

    /// <summary>
    /// Generates numerous download URLs for the speed test.
    /// </summary>
    /// <example>
    /// http://manchester.speedtest.boundlessnetworks.uk:8080/speedtest/random1500x1500.jpg?r=0
    /// http://manchester.speedtest.boundlessnetworks.uk:8080/speedtest/random1500x1500.jpg?r=1
    /// ...
    /// </example>
    private IEnumerable<string> GenerateDownloadUrls(string serverUrl)
    {
        var downloadUrl = GetBaseUrl(serverUrl).Append("random{0}x{0}.jpg?r={1}");

        foreach (var downloadSize in settings.DownloadSizes)
        {
            for (var i = 0; i < settings.DownloadSizeIterations; i++)
            {
                yield return string.Format(downloadUrl, downloadSize, i);
            }
        }
    }

    private static string GetBaseUrl(string url)
    {
        return new Uri(new Uri(url), ".").OriginalString;
    }

    private static HttpClient GetHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html, application/xhtml+xml, */*");
        httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        return httpClient;
    }

    #endregion
}