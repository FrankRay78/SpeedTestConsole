using Microsoft.Extensions.DependencyInjection;
using SpeedTestConsole.DependencyInjection;

var registrations = new ServiceCollection();
registrations.AddSingleton<ISpeedTestClient, SpeedTestClient>();
var registrar = new TypeRegistrar(registrations);

var app = new CommandApp(registrar);

app.Configure(CommandAppHelper.ConfigureAction);

var result = app.Run(args);

return result;