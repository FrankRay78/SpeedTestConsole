using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

internal class CustomHelpProvider : HelpProvider
{
    /// <summary>
    /// Used to provide a description for the application root.
    /// </summary>
    private class DummyCommand : ICommandInfo
    {
        public string Name => throw new NotImplementedException();

        public string? Description { get; set; }

        public bool IsBranch => throw new NotImplementedException();

        public bool IsDefaultCommand => throw new NotImplementedException();

        public bool IsHidden => throw new NotImplementedException();

        public IReadOnlyList<ICommandParameter> Parameters => throw new NotImplementedException();

        public ICommandInfo? Parent => throw new NotImplementedException();

        public IReadOnlyList<string[]> Examples => throw new NotImplementedException();

        public IReadOnlyList<ICommandInfo> Commands => throw new NotImplementedException();

        public ICommandInfo? DefaultCommand => throw new NotImplementedException();
    }

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
            new FigletText(font, "SpeedTestConsole")
                .LeftJustified()
                .Color(Color.Gold1),
            Text.NewLine
        ];
    }

    public override IEnumerable<IRenderable> GetDescription(ICommandModel model, ICommandInfo? command)
    {
        if (command is null)
        {
            command = new DummyCommand { Description = "Internet speed tester including server discovery, latency measurement, and download speed testing." };
        }

        return base.GetDescription(model, command);
    }
}