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

        var result = await speedTestClient.GetDownloadSpeedAsync(fastest.Value.server);

        Console.WriteLine($"Download: {result.DownloadSpeed} {result.SpeedUnit}");

        return 0;
    }
}