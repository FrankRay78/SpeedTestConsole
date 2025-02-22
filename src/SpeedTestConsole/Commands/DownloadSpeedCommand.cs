using ByteSizeLib;
using Humanizer;

namespace SpeedTestConsole.Commands;

public sealed class DownloadSpeedCommand : AsyncCommand<DownloadSpeedCommandSettings>
{
    private IAnsiConsole console;
    private ISpeedTestClient speedTestClient;
    private IClock clock;

    public DownloadSpeedCommand(IAnsiConsole console, ISpeedTestClient speedTestClient, IClock clock)
    {
        this.console = console;
        this.speedTestClient = speedTestClient;
        this.clock = clock;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, DownloadSpeedCommandSettings settings)
    {
        var servers = await speedTestClient.GetServersAsync();


        var fastest = await speedTestClient.GetFastestServerByLatencyAsync(servers);

        if (fastest == null)
        {
            throw new Exception("No servers available");
        }

        console.WriteLine($"Fastest server: {fastest.Value.server.Sponsor} ({fastest.Value.latency}ms)");


        SpeedTestResult result = new SpeedTestResult();

        // Show download progress
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
                var downloadProgressTask = progress.AddTask("Downloading", autoStart: true, maxValue: 100);

                Action<int> UpdateProgress = (int percentageComplete) =>
                {
                    // Update the progress bar
                    downloadProgressTask.Value = percentageComplete;
                };

                result = await speedTestClient.GetDownloadSpeedAsync(fastest.Value.server, UpdateProgress);

            });


        var size = ByteSize.FromBytes(result.BytesProcessed);
        var elapsed = TimeSpan.FromMilliseconds(result.ElapsedMilliseconds);

        console.WriteLine($"{size.ToString()} downloaded in {elapsed.Humanize()}");

        var speedString = result.GetSpeedString(settings.SpeedUnit);

        if (settings.IncludeTimestamp)
        {
            console.Write($"{clock.Now.ToString(settings.DateTimeFormat)} ");
        }
        console.WriteLine($"Speed: {speedString} download");


        return 0;
    }
}