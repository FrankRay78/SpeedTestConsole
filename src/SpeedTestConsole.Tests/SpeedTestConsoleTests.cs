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
}
