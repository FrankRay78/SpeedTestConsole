namespace SpeedTestConsole;

internal static class CommandAppHelper
{
    public static Action<IConfigurator> ConfigureAction = (config =>
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
}
