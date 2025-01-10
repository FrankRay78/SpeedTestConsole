using ByteSizeLib;
using SpeedTestConsole.Lib.Client;
using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Commands;

public sealed class DownloadSpeedCommand : AsyncCommand<DownloadSpeedCommandSettings>
{
    private IAnsiConsole console;
    private ISpeedTestClient speedTestClient;

    public DownloadSpeedCommand(IAnsiConsole console, ISpeedTestClient speedTestClient)
    {
        this.console = console;
        this.speedTestClient = speedTestClient;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, DownloadSpeedCommandSettings settings)
    {
        var servers = await speedTestClient.GetServersAsync();


        var fastest = await speedTestClient.GetFastestServerByLatencyAsync(servers);

        if (fastest == null)
        {
            console.MarkupLine("[red]No servers available[/]");
            return 1;
        }

        Console.WriteLine($"Fastest server: {fastest.Value.server.Sponsor} ({fastest.Value.latency}ms)");


        (long bytesProcessed, long elapsedMilliseconds) result = (0, 0);

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


        Console.WriteLine($"{result.bytesProcessed} bytes downloaded in {result.elapsedMilliseconds} ms");

        // Calculate the download speed
        var sizePerSecond = ByteSize.FromBytes(result.bytesProcessed / ((double)result.elapsedMilliseconds / 1000));

        Console.WriteLine($"Speed: {sizePerSecond.ToString()}/s");

        return 0;
    }
}