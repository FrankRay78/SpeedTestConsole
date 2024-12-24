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

        await speedTestClient.GetServerLatencyAsync(servers, true);

        var server = servers.GetFastestServerByLatency();

        if (server == null)
        {
            console.MarkupLine("[red]No servers available[/]");
            return 1;
        }

        Console.WriteLine($"Fastest server: {server.Sponsor} ({server.Latency}ms)");

        return 0;

        var result = await speedTestClient.GetDownloadSpeedAsync(server);

        Console.WriteLine($"Download: {result.DownloadSpeed} {result.SpeedUnit}");

        return 0;
    }
}