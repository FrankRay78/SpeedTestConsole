using System.Diagnostics;
using System.Net.Http.Headers;

namespace SpeedTestConsole.Library.Client;

/// <summary>
/// An Ookla Speedtest implementation of the <see cref="ISpeedTestService"/> interface.
/// </summary>
public sealed class OoklaSpeedtest : ISpeedTestService
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

        var downloadUrls = GenerateDownloadUrls(server.Url, settings.DownloadSizes, settings.DownloadSizeIterations);

        // Download content from a specified URL and return the size of the data in bytes.
        Func<HttpClient, string, Task<int>> DownloadAndMeasureAsync = async (client, url) =>
        {
            var data = await client.GetStringAsync(url).ConfigureAwait(false);
            return data.Length;
        };

        var downloadResult = await GenericTestSpeedAsync(downloadUrls, DownloadAndMeasureAsync, UpdateProgress, settings.DownloadParallelTasks);

        return downloadResult;
    }

    public async Task<SpeedTestResult> GetUploadSpeedAsync(IServer server)
    {
        return await GetUploadSpeedAsync(server, (int _) => { });
    }

    public async Task<SpeedTestResult> GetUploadSpeedAsync(IServer server, Action<int> UpdateProgress)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes a generic speed test by processing a collection of test data in parallel, 
    /// measuring total bytes processed and elapsed time.
    /// </summary>
    private async Task<SpeedTestResult> GenericTestSpeedAsync<T>(
        IEnumerable<T> testData,
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

        // Create and execute tasks to process the test data in parallel.
        var tasks = testData.Select(async data =>
        {
            // Limit concurrent executions by waiting for a permit from the semaphore.
            await throttler.WaitAsync().ConfigureAwait(false);

            using var httpClient = GetHttpClient();
            try
            {
                // Perform the work and retrieve the processed byte count.
                var size = await doWork(httpClient, data).ConfigureAwait(false);

                // Safely update the progress count and report completion percentage.
                lock (lockObject)
                {
                    completedCount++;
                    int percentageComplete = (int)(((double)completedCount / totalCount) * 100);
                    UpdateProgress(percentageComplete);
                }

                return size;
            }
            finally
            {
                // Release the semaphore to allow another task to proceed.
                throttler.Release();
            }
        }).ToArray();

        // Wait for all tasks to complete.
        await Task.WhenAll(tasks);
        timer.Stop();

        // Compute the total bytes processed.
        long totalBytesProcessed = tasks.Sum(task => task.Result);

        return new SpeedTestResult
        {
            BytesProcessed = totalBytesProcessed,
            ElapsedMilliseconds = timer.ElapsedMilliseconds
        };
    }

    #region Static Helper Functions

    /// <summary>
    /// Generates numerous download URLs for the speed test.
    /// </summary>
    /// <example>
    /// http://manchester.speedtest.boundlessnetworks.uk:8080/speedtest/random1500x1500.jpg?r=0
    /// http://manchester.speedtest.boundlessnetworks.uk:8080/speedtest/random1500x1500.jpg?r=1
    /// ...
    /// </example>
    private static IEnumerable<string> GenerateDownloadUrls(string serverUrl, int[] downloadSizes, int downloadSizeIterations)
    {
        var downloadUrl = GetBaseUrl(serverUrl).Append("random{0}x{0}.jpg?r={1}");

        foreach (var downloadSize in downloadSizes)
        {
            for (var i = 0; i < downloadSizeIterations; i++)
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