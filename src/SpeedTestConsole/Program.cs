using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
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
        // Add the Serilog logger to the service collection
        var serviceCollection = new ServiceCollection()
            .AddLogging(configure =>
                configure.AddSerilog(new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}", theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: false)
                    .CreateLogger()
                )
            );

        //var registrar = new TypeRegistrar(serviceCollection);
        //registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        //registrar.Register(typeof(IClock), typeof(ClockStub));

        var registrar = new TypeRegistrar(serviceCollection);
        registrar.Register(typeof(OoklaSpeedtestSettings), typeof(OoklaSpeedtestSettings));
        registrar.Register(typeof(ISpeedTestService), typeof(OoklaSpeedtest));
        registrar.Register(typeof(IClock), typeof(Clock));

        var app = new CommandApp(registrar);

        app.Configure(ConfigureAction);

        var result = app.Run(args);

        return result;
    }
}