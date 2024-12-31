using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace SpeedTestConsole.Lib.Client;

public sealed class SpeedTestClient : ISpeedTestClient
{
    public TestStage CurrentStage { get; private set; } = TestStage.Stopped;
    public SpeedUnit SpeedUnit { get; private set; } = SpeedUnit.Kbps;

    public event EventHandler<TestStage>? StageChanged;
    public event EventHandler<ProgressInfo>? ProgressChanged;

    #region Frank

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

    public async Task<int?> GetServerLatencyAsync(Server server, int httpTimeoutMilliseconds)
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
        parallelDownloads);

        return new SpeedTestResult(settings.SpeedUnit, downloadSpeed, -1, -1);
    }

    private async Task<double> GenericTestSpeedAsync<T>(IEnumerable<T> testData,
        Func<HttpClient, T, Task<int>> doWork,
        int parallelTasks)
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
                    BytesProcessed = totalBytesProcessed,
                    Speed = ConvertUnit(totalBytesProcessed * 8.0 / 1024.0 /
                                           ((double)timer.ElapsedMilliseconds / 1000)),
                    TotalBytes = size
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
        return ConvertUnit(totalSize * 8 / 1024 / ((double)timer.ElapsedMilliseconds / 1000));
    }

    #region Helper Functions

    private double ConvertUnit(double value)
    {
        return SpeedUnit switch
        {
            SpeedUnit.Kbps => value,
            SpeedUnit.KBps => value / 8.0,
            SpeedUnit.Mbps => value / 1024.0,
            SpeedUnit.MBps => value / 8192.0,
            _ => throw new InvalidEnumArgumentException("Not a valid SpeedUnit")
        };
    }

    private IEnumerable<string> GenerateDownloadUrls(string serverUrl)
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

    #endregion






    public async Task<SpeedTestResult> TestSpeedAsync(SpeedUnit speedUnit,
        int parallelTasks = 8,
        bool testLatency = true,
        bool testDownload = true,
        bool testUpload = true)
    {
        if (CurrentStage != TestStage.Stopped)
        {
            throw new InvalidOperationException("Speedtest already running");
        }
        SpeedUnit = speedUnit;

        try
        {
            var server = await GetBestServerByLatency();

            if (server == null)
            {
                throw new InvalidOperationException("No server was found");
            }

            var latency = testLatency ? await TestServerLatencyAsync(server) : -1;
            var downloadSpeed = testDownload ? await TestDownloadSpeedAsync(server, parallelTasks) : -1;
            var uploadSpeed = testUpload ? await TestUploadSpeedAsync(server, parallelTasks) : -1;

            return new SpeedTestResult(speedUnit, downloadSpeed, uploadSpeed, latency);
        }
        finally
        {
            SetStage(TestStage.Stopped);
        }
    }

    private async Task<Server?> GetBestServerByLatency()
    {
        var servers = await FetchServersAsync();
        var serverLatency = new Dictionary<Server, int>();
        foreach (var server in servers)
        {
            try
            {
                var latency = await TestServerLatencyAsync(server);
                serverLatency.TryAdd(server, latency);
            }
            catch
            {
                // ignore this server
            }
        }

        return serverLatency.OrderBy(x => x.Value).Select(x => x.Key).FirstOrDefault();
    }

    private async Task<Server[]> FetchServersAsync()
    {
        using var httpClient = GetHttpClient();
        var serversXml = await httpClient.GetStringAsync(Constants.ServersUrl);
        return serversXml.DeserializeFromXml<ServersList>().Servers ?? Array.Empty<Server>();
    }

    private async Task<int> TestServerLatencyAsync(Server server, int tests = 4)
    {
        SetStage(TestStage.Latency);

        if (string.IsNullOrWhiteSpace(server.Url))
        {
            throw new NullReferenceException("Server url was null");
        }

        var latencyUrl = GetBaseUrl(server.Url).Append("latency.txt");
        var stopwatch = new Stopwatch();
        using var httpClient = GetHttpClient();

        var test = 1;
        do
        {
            stopwatch.Start();
            var testString = await httpClient.GetStringAsync(latencyUrl);
            stopwatch.Stop();

            if (!testString.StartsWith("test=test"))
            {
                throw new InvalidOperationException("Server returned incorrect test string for latency.txt");
            }
            test++;
        } while (test < tests);

        return (int)stopwatch.ElapsedMilliseconds / tests;
    }

    private async Task<double> TestUploadSpeedAsync(Server server, int parallelUploads)
    {
        SetStage(TestStage.Upload);

        if (string.IsNullOrWhiteSpace(server.Url))
        {
            throw new NullReferenceException("Server url was null");
        }

        var testData = GenerateUploadData();

        return await GenericTestSpeedAsync(testData, async (client, uploadData) =>
        {
            using var content = new ByteArrayContent(uploadData);
            await client.PostAsync(server.Url, content).ConfigureAwait(false);
            return uploadData.Length;
        }, parallelUploads);
    }

    private async Task<double> TestDownloadSpeedAsync(Server server, int parallelDownloads)
    {
        SetStage(TestStage.Download);

        if (string.IsNullOrWhiteSpace(server.Url))
        {
            throw new NullReferenceException("Server url was null");
        }

        var testData = GenerateDownloadUrls(server.Url);

        return await GenericTestSpeedAsync(testData, async (client, url) =>
        {
            var data = await client.GetStringAsync(url).ConfigureAwait(false);
            return data.Length;
        }, parallelDownloads);
    }



    private static IEnumerable<byte[]> GenerateUploadData()
    {
        var random = new Random();
        var result = new List<byte[]>();

        for (var sizeCounter = 1; sizeCounter < Constants.MaxUploadSize + 1; sizeCounter++)
        {
            var size = sizeCounter * 200 * 1024;
            var builder = new StringBuilder(size);

            for (var i = 0; i < size; ++i)
            {
                builder.Append(Constants.Chars[random.Next(Constants.Chars.Length)]);
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());

            for (var i = 0; i < 10; i++)
            {
                result.Add(bytes);
            }
        }

        return result;
    }

    private void SetStage(TestStage newStage)
    {
        if (CurrentStage == newStage)
        {
            return;
        }

        CurrentStage = newStage;
        StageChanged?.Invoke(this, newStage);
    }
}