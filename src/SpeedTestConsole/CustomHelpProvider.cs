using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

internal class CustomHelpProvider : HelpProvider
{
    public CustomHelpProvider(ICommandAppSettings settings)
        : base(settings)
    {
    }

    public override IEnumerable<IRenderable> GetHeader(ICommandModel model, ICommandInfo? command)
    {
        var font = FigletFont.Load("slant.flf");

        return
        [
            Text.NewLine,
            new FigletText(font, "Speedtest Console")
                .LeftJustified()
                .Color(Color.Gold1),
            Text.NewLine
        ];
    }
}