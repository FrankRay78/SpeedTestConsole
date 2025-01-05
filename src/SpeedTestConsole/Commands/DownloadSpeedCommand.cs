using ByteSizeLib;
using SpeedTestConsole.Lib.Client;
using SpeedTestConsole.Lib.Extensions;

namespace SpeedTestConsole.Commands;

public sealed class DownloadSpeedCommand : AsyncCommand
{
    private IAnsiConsole console;
    private ISpeedTestClient speedTestClient;

    public DownloadSpeedCommand(IAnsiConsole console, ISpeedTestClient speedTestClient)
    {
        this.console = console;
        this.speedTestClient = speedTestClient;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var servers = await speedTestClient.GetServersAsync();

        var fastest = await speedTestClient.GetFastestServerByLatencyAsync(servers);

        if (fastest == null)
        {
            console.MarkupLine("[red]No servers available[/]");
            return 1;
        }

        Console.WriteLine($"Fastest server: {fastest.Value.server.Sponsor} ({fastest.Value.latency}ms)");

        Action<int> UpdateProgress = (int percentageComplete) =>
        {
            // TODO: Update the progress bar

            console.MarkupLine($"Complete: {percentageComplete}%");
        };

        var result = await speedTestClient.GetDownloadSpeedAsync(fastest.Value.server, UpdateProgress);

        Console.WriteLine($"{result.bytesProcessed} bytes downloaded in {result.elapsedMilliseconds} ms");

        // Calculate the download speed
        var sizePerSecond = ByteSize.FromBytes(result.bytesProcessed / ((double)result.elapsedMilliseconds / 1000));

        Console.WriteLine($"Speed: {sizePerSecond.ToString()}/s");

        return 0;
    }
}