using System.ComponentModel;

namespace SpeedTestConsole.Commands;

public sealed class ListServersCommandSettings : CommandSettings
{
    [CommandOption("-l|--latency")]
    [Description("Include server latency.")]
    public bool? ShowLatency { get; set; } = false;
}
