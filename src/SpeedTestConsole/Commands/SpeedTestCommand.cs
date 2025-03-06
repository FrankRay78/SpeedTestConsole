using ByteSizeLib;
using Humanizer;

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
        var server = await GetFastestServerAsync(settings);


        var speedTestResult = await PerformSpeedTestAsync(server, settings);

        var downloadResult = speedTestResult.downloadResult;
        var uploadResult = speedTestResult.uploadResult;


        if ((settings.Verbosity & Verbosity.Debug) != 0)
        {
            ByteSize size; TimeSpan elapsed;

            size = ByteSize.FromBytes(downloadResult.BytesProcessed);
            elapsed = TimeSpan.FromMilliseconds(downloadResult.ElapsedMilliseconds);

            console.WriteLine($"{size.ToString()} downloaded in {elapsed.Humanize()}");

            size = ByteSize.FromBytes(uploadResult.BytesProcessed);
            elapsed = TimeSpan.FromMilliseconds(uploadResult.ElapsedMilliseconds);

            console.WriteLine($"{size.ToString()} uploaded in {elapsed.Humanize()}");
        }


        if (settings.IncludeTimestamp)
        {
            console.Write($"{clock.Now.ToString(settings.DateTimeFormat)} ");
        }

        console.Write($"Download: {downloadResult.GetSpeedString(settings.SpeedUnit)} ");
        console.Write($"Upload: {uploadResult.GetSpeedString(settings.SpeedUnit)}");


        return 0;
    }

    private async Task<IServer> GetFastestServerAsync(SpeedTestCommandSettings settings)
    {
        var servers = await speedTestClient.GetServersAsync();

        var fastest = await speedTestClient.GetFastestServerByLatencyAsync(servers);

        if (fastest == null)
        {
            throw new Exception("No servers available");
        }

        if ((settings.Verbosity & (Verbosity.Normal | Verbosity.Debug)) != 0)
        {
            console.WriteLine($"{fastest.Value.server.Sponsor} ({fastest.Value.latency} ms)");
        }

        return fastest.Value.server;
    }

    private async Task<(SpeedTestResult downloadResult, SpeedTestResult uploadResult)> PerformSpeedTestAsync(IServer server, SpeedTestCommandSettings settings)
    {
        var downloadResult = new SpeedTestResult();
        var uploadResult = new SpeedTestResult();

        if ((settings.Verbosity & Verbosity.Minimal) != 0)
        {
            downloadResult = await speedTestClient.GetDownloadSpeedAsync(server);
            uploadResult = await speedTestClient.GetUploadSpeedAsync(server);
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
                    var downloadProgress = progress.AddTask("Downloading", autoStart: true, maxValue: 100);
                    var uploadProgress = progress.AddTask("Uploading", autoStart: true, maxValue: 100);

                    downloadResult = await speedTestClient.GetDownloadSpeedAsync(server, (int percentageComplete) =>
                    {
                        // Update the download progress bar
                        downloadProgress.Value = percentageComplete;
                    });

                    uploadResult = await speedTestClient.GetUploadSpeedAsync(server, (int percentageComplete) =>
                    {
                        // Update the upload progress bar
                        uploadProgress.Value = percentageComplete;
                    });
                });
        }

        return (downloadResult, uploadResult);
    }
}