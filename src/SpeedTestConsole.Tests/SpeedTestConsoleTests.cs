using Microsoft.Extensions.DependencyInjection;
using SpeedTestConsole.DependencyInjection;
using System.Runtime.Serialization;

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
        var result = await app.RunAsync();

        // Then
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestClient), typeof(SpeedTestStub));

        var app = new CommandAppTester(registrar);
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("servers");

        // Then
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers_With_Latency()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestClient), typeof(SpeedTestStub));

        var app = new CommandAppTester(registrar);
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("servers", "-l");

        // Then
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Perform_Download_Speed_Test()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestClient), typeof(SpeedTestStub));

        var app = new CommandAppTester(registrar);
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download");

        // Then
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Handle_Unknown_Exceptions()
    {
        // Given
        var mock = new SpeedTestMock
        {
            GetServersAsyncFunc = () => throw new HttpRequestException("Could not open socket")
        };

        var registrar = new TypeRegistrar();
        registrar.RegisterInstance(typeof(ISpeedTestClient), mock);

        var app = new CommandAppTester(registrar);
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download");

        // Then
        await Verify(result.Output);
    }
}
