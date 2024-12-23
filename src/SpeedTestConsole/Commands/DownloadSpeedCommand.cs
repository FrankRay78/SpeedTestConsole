namespace SpeedTestConsole.Commands;

public sealed class DownloadSpeedCommand : AsyncCommand
{
    private IAnsiConsole console;

    public DownloadSpeedCommand(IAnsiConsole console)
    {
        this.console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        ISpeedTestClient speedTestClient = new SpeedTestClient();

        var result = await speedTestClient.GetDownloadSpeedAsync();

        Console.WriteLine($"Download: {result.DownloadSpeed} {result.SpeedUnit}");

        return 0;
    }
}