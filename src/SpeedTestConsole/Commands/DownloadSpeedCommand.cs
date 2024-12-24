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

        var server = await speedTestClient.GetFastestServerByLatency(servers);

        Console.WriteLine($"Fastest server: {server!.Sponsor}");

        return 0;

        var result = await speedTestClient.GetDownloadSpeedAsync(server);

        Console.WriteLine($"Download: {result.DownloadSpeed} {result.SpeedUnit}");

        return 0;
    }
}