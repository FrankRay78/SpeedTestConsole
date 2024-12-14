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
        //console.MarkupLine("Speedtest Servers");

        ISpeedTestClient speedTestClient = new SpeedTestClient();

        var servers = await speedTestClient.GetServersAsync();


        var table = new Table()
            .Title("Speedtest Servers")
            .Border(TableBorder.Square)
            .BorderColor(Color.Red)
            .AddColumn(new TableColumn("Country"))
            .AddColumn(new TableColumn("Sponsor"));

        foreach (var server in servers)
        {
            //console.MarkupLine($"Server: { server.Name } ({ server.Sponsor })");

            table.AddRow(server.Name ?? string.Empty, server.Sponsor ?? string.Empty);
        }

        //table.Rows[0].Cells[0].Style = new Style(foreground: Color.Green);

        //table.AddColumn(new TableColumn("Latency"));

        console.WriteLine("");
        console.Write(table);

        return 0;
    }
}