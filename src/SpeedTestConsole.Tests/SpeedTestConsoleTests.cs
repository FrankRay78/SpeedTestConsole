﻿using Spectre.Console.Cli;
using SpeedTestConsole.Commands;
using SpeedTestConsole.DependencyInjection;

namespace SpeedTestConsole.Tests;

public class SpeedTestConsoleTests
{
    /// <summary>
    /// Create the CommandAppTester and configure.
    /// </summary>
    private static CommandAppTester GetCommandAppTester(ITypeRegistrar? registrar = null)
    {
        var app = registrar == null ? 
            new CommandAppTester(new CommandAppTesterSettings { TrimConsoleOutput = false }) :
            new CommandAppTester(registrar, new CommandAppTesterSettings { TrimConsoleOutput = false });
        app.SetDefaultCommand<SpeedTestCommand>("Internet speed tester including server discovery, latency measurement, download and upload speed testing.");
        app.Configure(Program.ConfigureAction);
        return app;
    }

    [Fact]
    public async Task Should_Display_Speed_Test_Servers()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        var app = GetCommandAppTester(registrar);

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
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync("servers", "-l");

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Handle_No_Servers_Available()
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
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync();

        // Then
        Assert.Equal(-1, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Perform_Speed_Test()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync();

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [InlineData("Minimal")]
    [InlineData("Normal")]
    [InlineData("Debug")]
    [Theory]
    public async Task Should_Perform_Speed_Test_With_Verbosity(string verbosity)
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync("--verbosity", verbosity);

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output).UseParameters(verbosity);
    }

    [InlineData("-t")]
    [InlineData("--timestamp")]
    [Theory]
    public async Task Should_Perform_Speed_Test_With_Timestamp(string timestamp)
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync(timestamp);

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output).DisableRequireUniquePrefix();
    }

    [Fact]
    public async Task Should_Not_Perform_Download_Speed_Test()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync("--no-download");

        // Then
        Assert.Equal(0, result.ExitCode);
        await Verify(result.Output);
    }

    [Fact]
    public async Task Should_Not_Perform_Upload_Speed_Test()
    {
        // Given
        var registrar = new TypeRegistrar();
        registrar.Register(typeof(ISpeedTestService), typeof(SpeedTestStub));
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync("--no-upload");

        // Then
        Assert.Equal(0, result.ExitCode);
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
        registrar.RegisterInstance(typeof(ISpeedTestService), mock);
        registrar.Register(typeof(IClock), typeof(ClockStub));
        var app = GetCommandAppTester(registrar);

        // When
        var result = await app.RunAsync();

        // Then
        Assert.Equal(-1, result.ExitCode);
        await Verify(result.Output);
    }
}
