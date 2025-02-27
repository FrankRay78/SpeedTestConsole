﻿using System.ComponentModel;

namespace SpeedTestConsole.Commands;

public sealed class DownloadSpeedCommandSettings : CommandSettings
{
    [CommandOption("-t | --timestamp")]
    [Description("Include a timestamp.")]
    [DefaultValue(false)]
    public bool IncludeTimestamp { get; set; }

    [CommandOption("--datetimeformat", IsHidden=true)]
    [Description("The date and time format string (as defined by Microsoft.Net).")]
    [DefaultValue("yyyy'-'MM'-'dd' 'HH':'mm':'ss")]
    //ref: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
    public string? DateTimeFormat { get; set; }

    [CommandOption("-u | --unit")]
    [Description("The speed unit.\nBitsPerSecond, BytesPerSecond")]
    [DefaultValue(SpeedUnit.BitsPerSecond)]
    public SpeedUnit SpeedUnit { get; set; }
}
