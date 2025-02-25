using SpeedTestConsole;
using SpeedTestConsole.DependencyInjection;

public static class Program
{
    /// <summary>
    /// The configure action for the CommandApp.
    /// </summary>
    /// <remarks>
    /// Extracted here so the testing project can reuse the production configuration.
    /// </remarks>
    internal static Action<IConfigurator> ConfigureAction = (config =>
    {
        config.SetApplicationName("SpeedTestConsole");
        config.ValidateExamples();
        config.Settings.ShowOptionDefaultValues = true;
        config.Settings.TrimTrailingPeriod = false;

        // Register the custom help provider
        config.SetHelpProvider(new CustomHelpProvider(config.Settings));

        config.AddCommand<ListServersCommand>("servers").WithDescription("Show the nearest speed test servers.");
        config.AddCommand<DownloadSpeedCommand>("download").WithDescription("Perform an internet download speed test.");
    });

    public static int Main(string[] args)
    {
        //var registrar = new TypeRegistrar();
        //registrar.Register(typeof(ISpeedTestClient), typeof(SpeedTestStub));
        //registrar.Register(typeof(IClock), typeof(ClockStub));

        var registrar = new TypeRegistrar();
        registrar.Register(typeof(OoklaSpeedtestSettings), typeof(OoklaSpeedtestSettings));
        registrar.Register(typeof(ISpeedTestClient), typeof(OoklaSpeedtest));
        registrar.Register(typeof(IClock), typeof(Clock));

        var app = new CommandApp(registrar);

        app.Configure(ConfigureAction);

        var result = app.Run(args);

        return result;
    }
}