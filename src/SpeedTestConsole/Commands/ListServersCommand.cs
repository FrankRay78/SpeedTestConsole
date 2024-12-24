namespace SpeedTestConsole.Commands;

public sealed class ListServersCommand : AsyncCommand<ListServersCommandSettings>
{
    private IAnsiConsole console;
    private ISpeedTestClient speedTestClient;

    public ListServersCommand(IAnsiConsole console, ISpeedTestClient speedTestClient)
    {
        this.console = console;
        this.speedTestClient = speedTestClient;

    }

    public override async Task<int> ExecuteAsync(CommandContext context, ListServersCommandSettings settings)
    {
        var servers = await speedTestClient.GetServersAsync();

        var serversList = servers.OrderBy(servers => servers.Name).ToList();

        if (settings.ShowLatency == null || !settings.ShowLatency.HasValue || !settings.ShowLatency.Value)
        {
            DisplayServers(serversList);
        }
        else
        {
            await DisplayServersWithLatency(serversList, speedTestClient);
        }

        return 0;
    }

    private void DisplayServers(List<Server> servers)
    {
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
    }

    private async Task DisplayServersWithLatency(List<Server> servers, ISpeedTestClient speedTestClient)
    {
        var table = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Red)
            .AddColumn(new TableColumn("Country"))
            .AddColumn(new TableColumn("Sponsor"))
            .AddColumn(new TableColumn("Latency"));

        // Add the initial server list (without latency)
        foreach (var server in servers)
        {
            table.AddRow(server.Name ?? string.Empty, server.Sponsor ?? string.Empty);
        }

        console.MarkupLine("Press [yellow]CTRL+C[/] to exit");

        await AnsiConsole.Live(table)
            .AutoClear(false)
            .StartAsync(async ctx =>
            {
                // Fetch the latency for each server
                // and update the table as they come back
                for (int i = 0; i < servers.Count; i++)
                {
                    try
                    {
                        var latency = await speedTestClient.GetServerLatencyAsync(servers[i]);

                        table.UpdateCell(i, 2, $"{latency}ms");
                    }
                    catch (Exception)
                    {
                        table.UpdateCell(i, 2, "-");
                    }

                    ctx.Refresh();
                }
            });
    }
}