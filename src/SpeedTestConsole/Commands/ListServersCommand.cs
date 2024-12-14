namespace SpeedTestConsole.Commands;

public sealed class ListServersCommand : AsyncCommand
{
    private IAnsiConsole console;

    public ListServersCommand(IAnsiConsole console)
    {
        this.console = console;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        ISpeedTestClient speedTestClient = new SpeedTestClient();

        var servers = await speedTestClient.GetServersAsync();


        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Red)
            .AddColumn(new TableColumn("Country"))
            .AddColumn(new TableColumn("Sponsor"));

        foreach (var server in servers)
        {
            table.AddRow(server.Name ?? string.Empty, server.Sponsor ?? string.Empty);
        }

        console.WriteLine("");
        console.Write(table);

        return 0;
    }
}