using Microsoft.Extensions.DependencyInjection;
using SpeedTestConsole.DependencyInjection;
using SpeedTestConsole.Testing;

namespace SpeedTestConsole.Tests;

public class SpeedTestConsoleTests
{
    [Fact]
    public async Task Should_Display_Help_When_Run_With_No_Arguments()
    {
        // Given
        var app = new CommandAppTester();
        app.Configure(Program.ConfigureAction);

        // When
        var result = app.Run();

        // Then
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers()
    {
        // Given
        var registrations = new ServiceCollection();
        registrations.AddSingleton<ISpeedTestClient, SpeedTestStub>();
        var registrar = new TypeRegistrar(registrations);

        var app = new CommandAppTester(registrar);
        app.Configure(Program.ConfigureAction);

        // When
        var result = app.Run("servers");

        // Then
        await Verify(result.Output);
    }
}
