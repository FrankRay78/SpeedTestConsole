using Microsoft.Extensions.DependencyInjection;
using SpeedTestConsole.DependencyInjection;

var registrations = new ServiceCollection();
registrations.AddSingleton<ISpeedTestClient, SpeedTestClient>();
var registrar = new TypeRegistrar(registrations);

var app = new CommandApp(registrar);

app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif

    config.SetApplicationName("SpeedTestConsole");

    // Register the custom help provider
    config.SetHelpProvider(new CustomHelpProvider(config.Settings));

    config.AddCommand<ListServersCommand>("servers");
    config.AddCommand<DownloadSpeedCommand>("download");
});

var result = app.Run(args);

return result;