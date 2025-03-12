﻿using System.ComponentModel;

namespace SpeedTestConsole.Commands;

public sealed class SpeedTestCommandSettings : CommandSettings
{
    [CommandOption("--csv")]
    [Description("Display minimal output in CSV format (always includes timestamp).")]
    [DefaultValue(false)]
    public bool CSV { get; set; }

    [CommandOption("--csv-delimiter")]
    [Description("Single character delimiter to use in CSV output.")]
    [DefaultValue(',')]
    public char CSVDelimiter { get; set; }

    [CommandOption("--no-download")]
    [Description("Do not perform download test.")]
    [DefaultValue(false)]
    public bool NoDownload { get; set; }

    [CommandOption("--no-upload")]
    [Description("Do not perform upload test.")]
    [DefaultValue(false)]
    public bool NoUpload { get; set; }

    [CommandOption("-t | --timestamp")]
    [Description("Include a timestamp.")]
    [DefaultValue(false)]
    public bool IncludeTimestamp { get; set; }

    [CommandOption("--datetimeformat", IsHidden = true)]
    [Description("The datetime format string (as defined by Microsoft.Net).")]
    [DefaultValue("yyyy'-'MM'-'dd' 'HH':'mm':'ss")]
    //ref: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
    public string? DateTimeFormat { get; set; }

    [CommandOption("-u | --unit")]
    [Description("The speed unit. <BitsPerSecond, BytesPerSecond>")]
    [DefaultValue(SpeedUnit.BitsPerSecond)]
    public SpeedUnit SpeedUnit { get; set; }

    [CommandOption("--unit-system")]
    [Description("The speed unit system. <SI, IEC>\nSI steps up in powers of 1000 (KB, MB, GB), common in networking, while IEC uses powers of 1024 (KiB, MiB, GiB), standard in computing and storage.")]
    [DefaultValue(SpeedUnitSystem.SI)]
    public SpeedUnitSystem SpeedUnitSystem { get; set; }

    [CommandOption("--verbosity")]
    [Description("The verbosity level. <Minimal, Normal, Debug>\nMinimal is ideal for batch scripts and redirected output.")]
    [DefaultValue(Verbosity.Normal)]
    public Verbosity Verbosity { get; set; }
}
