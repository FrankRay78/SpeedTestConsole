using SpeedTestConsole.DependencyInjection;

namespace SpeedTestConsole.Tests;

public class SpeedTestConsoleTests
{
    [Fact]
    public async Task Should_Display_Help_When_Run_With_No_Arguments()
    {
        // Given
        var app = new CommandAppTester(new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync();

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("servers");

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers_With_Latency()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("servers", "-l");

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Handle_No_Download_Servers_Available_Test()
    {
        // Given
        var mock = new SpeedTestMock
        {
            GetServersAsyncFunc = () => Task.FromResult(Array.Empty<IServer>()),
            GetFastestServerByLatencyAsyncFunc = servers => Task.FromResult<(IServer server, int latency)?>(null),
        };

        var registrar = new TypeRegistrar();
        registrar.RegisterInstance(typeof(ISpeedTestService), mock);
        registrar.Register(typeof(IClock), typeof(ClockStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download");

        // Then
        Assert.Equal(-1, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Perform_Download_Speed_Test()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download");

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [InlineData("Minimal")]
    [InlineData("Normal")]
    [InlineData("Debug")]
    [Theory]
    public async Task Should_Perform_Download_Speed_Test_With_Verbosity(string verbosity)
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download", "--verbosity", verbosity);

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output).UseParameters(verbosity);
    }

    [InlineData("-t")]
    [InlineData("--timestamp")]
    [Theory]
    public async Task Should_Perform_Download_Speed_Test_With_Timestamp(string timestamp)
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download", timestamp);

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output).DisableRequireUniquePrefix();
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
        registrar.RegisterInstance(typeof(ISpeedTestService), mock);
        registrar.Register(typeof(IClock), typeof(ClockStub));

        var app = new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.Configure(Program.ConfigureAction);

        // When
        var result = await app.RunAsync("download");

        // Then
        Assert.Equal(-1, result.ExitCode);
        await Verify(result.Output);
    }
}
