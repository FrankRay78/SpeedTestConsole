using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SpeedTestConsole.Lib.Client;

public sealed class SpeedTestClient : ISpeedTestClient
{
    public event EventHandler<ProgressInfo>? ProgressChanged;

    private Settings settings;

    public SpeedTestClient(Settings? settings = null)
    {
        // Dependecy inject the settings to allow unit testing
        this.settings = settings ?? new Settings();
    }

    public async Task<Server[]> GetServersAsync()
    {
        using var httpClient = GetHttpClient();
        var serversXml = await httpClient.GetStringAsync(settings.ServersUrl);
        return serversXml.DeserializeFromXml<ServersList>().Servers ?? Array.Empty<Server>();
    }

    public async Task<int?> GetServerLatencyAsync(Server server)
    {
        return await GetServerLatencyAsync(server, settings.DefaultHttpTimeoutMilliseconds);
    }

    private async Task<int?> GetServerLatencyAsync(Server server, int httpTimeoutMilliseconds)
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


            var maximumIterations = settings.ServerLatencyIterations;
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
            while (iteration < maximumIterations);

            // Calculate the average server latency
            latency = (int)stopwatch.ElapsedMilliseconds / maximumIterations;
        }
        catch
        {
            // Ignore this server
        }

        return latency;
    }

    public async Task<(Server server, int latency)?> GetFastestServerByLatencyAsync(Server[] servers)
    {
        int fastestLatency = settings.DefaultHttpTimeoutMilliseconds;
        Server? fastestServer = null;

        foreach (var server in servers)
        {
            // nb. Bump up the fastest latency/timeout by a slight margin
            var httpTimeoutMilliseconds = (fastestLatency == settings.DefaultHttpTimeoutMilliseconds ? fastestLatency : (int)(fastestLatency * 1.5));

            var latency = await GetServerLatencyAsync(server, httpTimeoutMilliseconds);

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

    public async Task<SpeedTestResult> GetDownloadSpeedAsync(Server server)
    {
        if (string.IsNullOrWhiteSpace(server.Url))
        {
            throw new NullReferenceException("Server url was null");
        }

        int parallelDownloads = settings.DownloadParallelTasks;

        var testData = GenerateDownloadUrls(server.Url);

        var downloadSpeed = await GenericTestSpeedAsync(testData, async (client, url) =>
        {
            var data = await client.GetStringAsync(url).ConfigureAwait(false);
            return data.Length;
        },
        parallelDownloads,
        settings.SpeedUnit);

        return new SpeedTestResult(settings.SpeedUnit, downloadSpeed, -1, -1);
    }

    private async Task<double> GenericTestSpeedAsync<T>(IEnumerable<T> testData,
        Func<HttpClient, T, Task<int>> doWork,
        int parallelTasks, SpeedUnit speedUnit)
    {
        var timer = new Stopwatch();
        var throttler = new SemaphoreSlim(parallelTasks);

        timer.Start();
        long totalBytesProcessed = 0;

        var downloadTasks = testData.Select(async data =>
        {
            await throttler.WaitAsync().ConfigureAwait(false);
            using var httpClient = GetHttpClient();
            try
            {
                var size = await doWork(httpClient, data).ConfigureAwait(false);

                Interlocked.Add(ref totalBytesProcessed, size);
                var progressInfo = new ProgressInfo
                {
                    TotalBytesProcessed = totalBytesProcessed,
                    Speed = ConvertUnit(speedUnit, totalBytesProcessed * 8.0 / 1024.0 / ((double)timer.ElapsedMilliseconds / 1000)),
                    BytesProcessed = size
                };

                ProgressChanged?.Invoke(this, progressInfo);

                return size;
            }
            finally
            {
                throttler.Release();
            }
        }).ToArray();

        await Task.WhenAll(downloadTasks);
        timer.Stop();

        double totalSize = downloadTasks.Sum(task => task.Result);
        return ConvertUnit(speedUnit, totalSize * 8 / 1024 / ((double)timer.ElapsedMilliseconds / 1000));
    }

    #region Helper Functions

    private static double ConvertUnit(SpeedUnit speedUnit, double value)
    {
        return speedUnit switch
        {
            SpeedUnit.Kbps => value,
            SpeedUnit.KBps => value / 8.0,
            SpeedUnit.Mbps => value / 1024.0,
            SpeedUnit.MBps => value / 8192.0,
            _ => throw new InvalidEnumArgumentException("Not a valid SpeedUnit")
        };
    }

    private static IEnumerable<string> GenerateDownloadUrls(string serverUrl)
    {
        var downloadUrl = GetBaseUrl(serverUrl).Append("random{0}x{0}.jpg?r={1}");

        foreach (var downloadSize in Constants.DownloadSizes)
        {
            for (var i = 0; i < 4; i++)
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