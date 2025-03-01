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


        SpeedTestResult result = new SpeedTestResult();

        if ((settings.Verbosity & Verbosity.Minimal) != 0)
        {
            result = await speedTestClient.GetDownloadSpeedAsync(fastest.Value.server);
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
                    var downloadProgressTask = progress.AddTask("Downloading", autoStart: true, maxValue: 100);

                    Action<int> UpdateProgress = (int percentageComplete) =>
                    {
                        // Update the progress bar
                        downloadProgressTask.Value = percentageComplete;
                    };

                    result = await speedTestClient.GetDownloadSpeedAsync(fastest.Value.server, UpdateProgress);
                });
        }


        var size = ByteSize.FromBytes(result.BytesProcessed);
        var elapsed = TimeSpan.FromMilliseconds(result.ElapsedMilliseconds);

        if ((settings.Verbosity & Verbosity.Debug) != 0)
        {
            console.WriteLine($"{size.ToString()} downloaded in {elapsed.Humanize()}");
        }

        var speedString = result.GetSpeedString(settings.SpeedUnit);

        if (settings.IncludeTimestamp)
        {
            console.Write($"{clock.Now.ToString(settings.DateTimeFormat)} ");
        }
        console.WriteLine($"Download: {speedString}");


        return 0;
    }
}
