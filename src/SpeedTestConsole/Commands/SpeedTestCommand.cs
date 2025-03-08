using ByteSizeLib;
using Humanizer;
using System.Diagnostics.CodeAnalysis;

namespace SpeedTestConsole.Commands;

public sealed class SpeedTestCommand : AsyncCommand<SpeedTestCommandSettings>
{
    private IAnsiConsole console;
    private ISpeedTestService speedTestClient;
    private IClock clock;

    public SpeedTestCommand(IAnsiConsole console, ISpeedTestService speedTestClient, IClock clock)
    {
        this.console = console;
        this.speedTestClient = speedTestClient;
        this.clock = clock;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, SpeedTestCommandSettings settings)
    {
        // Get the speed test server
        var fastest = await GetFastestServerAsync(settings);

        if (!settings.CSV && (settings.Verbosity & (Verbosity.Normal | Verbosity.Debug)) != 0)
        {
            console.WriteLine($"{fastest.server.Sponsor} ({fastest.latency} ms)");
        }

        if (settings.NoDownload && settings.NoUpload)
        {
            return 0;
        }


        // Perform speed test
        var (downloadResult, uploadResult) = await PerformSpeedTestAsync(fastest.server, settings);


        // CSV output overrides the display options below
        if (settings.CSV)
        {
            console.WriteLine(string.Join(settings.CSVDelimiter, "Timestamp", "Download", "Upload"));
            console.WriteLine(string.Join(settings.CSVDelimiter, 
                clock.Now.ToString(settings.DateTimeFormat), 
                downloadResult.GetSpeedString(settings.SpeedUnit), 
                uploadResult.GetSpeedString(settings.SpeedUnit)));

            return 0;
        }


        if ((settings.Verbosity & Verbosity.Debug) != 0)
        {
            // Display detailed diagnostics
            ByteSize size; TimeSpan elapsed;

            if (!settings.NoDownload)
            {
                size = ByteSize.FromBytes(downloadResult.BytesProcessed);
                elapsed = TimeSpan.FromMilliseconds(downloadResult.ElapsedMilliseconds);
                console.WriteLine($"{size.ToString()} downloaded in {elapsed.Humanize()}");
            }
            if (!settings.NoUpload)
            {
                size = ByteSize.FromBytes(uploadResult.BytesProcessed);
                elapsed = TimeSpan.FromMilliseconds(uploadResult.ElapsedMilliseconds);
                console.WriteLine($"{size.ToString()} uploaded in {elapsed.Humanize()}");
            }
        }


        // Display speed test result
        if (settings.IncludeTimestamp)
        {
            console.Write($"{clock.Now.ToString(settings.DateTimeFormat)} ");
        }
        if (!settings.NoDownload)
        {
            console.Write($"Download: {downloadResult.GetSpeedString(settings.SpeedUnit)} ");
        }
        if (!settings.NoUpload)
        {
            console.Write($"Upload: {uploadResult.GetSpeedString(settings.SpeedUnit)}");
        }
        console.WriteLine("");


        return 0;
    }

    private async Task<(IServer server, int latency)> GetFastestServerAsync(SpeedTestCommandSettings settings)
    {
        var servers = await speedTestClient.GetServersAsync();
        var fastest = await speedTestClient.GetFastestServerByLatencyAsync(servers);

        if (fastest == null)
        {
            throw new Exception("No servers available");
        }

        return fastest.Value;
    }

    private async Task<(SpeedTestResult downloadResult, SpeedTestResult uploadResult)> PerformSpeedTestAsync(IServer server, SpeedTestCommandSettings settings)
    {
        var downloadResult = new SpeedTestResult();
        var uploadResult = new SpeedTestResult();

        if ((settings.Verbosity & Verbosity.Minimal) != 0)
        {
            if (!settings.NoDownload) downloadResult = await speedTestClient.GetDownloadSpeedAsync(server);
            if (!settings.NoUpload) uploadResult = await speedTestClient.GetUploadSpeedAsync(server);
        }
        else
        {
            // Graphical progress bar
            await console.Progress()
                .AutoClear(false)
                .Columns(
                [
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                ])
                .StartAsync(async progress =>
                {
                    ProgressTask? downloadProgress = null; ProgressTask? uploadProgress = null;

                    // Create the progress bars
                    if (!settings.NoDownload)
                    {
                        downloadProgress = progress.AddTask("Downloading", autoStart: true, maxValue: 100);
                    }
                    if (!settings.NoUpload)
                    {
                        uploadProgress = progress.AddTask("Uploading", autoStart: true, maxValue: 100);
                    }

                    // Perform the speed tests and show progress
                    if (!settings.NoDownload)
                    {
                        downloadResult = await speedTestClient.GetDownloadSpeedAsync(server, (int percentageComplete) =>
                        {
                            downloadProgress!.Value = percentageComplete;
                        });
                    }
                    if (!settings.NoUpload)
                    {
                        uploadResult = await speedTestClient.GetUploadSpeedAsync(server, (int percentageComplete) =>
                        {
                            uploadProgress!.Value = percentageComplete;
                        });
                    }
                });
        }

        return (downloadResult, uploadResult);
    }
}