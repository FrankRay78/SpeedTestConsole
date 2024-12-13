namespace SpeedTestConsole.Commands;

public sealed class ListServersCommand : Command
{
    public override int Execute(CommandContext context)
    {
        AnsiConsole.MarkupLine("ListServersCommand.Execute");
        return 0;
    }
}