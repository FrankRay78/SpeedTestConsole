using Microsoft.Extensions.DependencyInjection;
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
#if DEBUG
        config.PropagateExceptions();
        config.ValidateExamples();
#endif

        config.SetApplicationName("SpeedTestConsole");

        // Register the custom help provider
        config.SetHelpProvider(new CustomHelpProvider(config.Settings));

        config.AddCommand<ListServersCommand>("servers").WithDescription("Show the nearest speed test servers");
        config.AddCommand<DownloadSpeedCommand>("download").WithDescription("Perform an internet download speed test");
    });

    public static int Main(string[] args)
    {
        var registrations = new ServiceCollection();
        
        registrations.AddSingleton<ISpeedTestClient, SpeedTestStub>();
        //registrations.AddSingleton<ISpeedTestClient, SpeedTestClient>();

        var registrar = new TypeRegistrar(registrations);

        var app = new CommandApp(registrar);

        app.Configure(ConfigureAction);

        var result = app.Run(args);

        return result;
    }
}