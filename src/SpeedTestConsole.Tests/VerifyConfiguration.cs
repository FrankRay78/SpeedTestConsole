﻿using System.Runtime.CompilerServices;

namespace SpeedTestConsole.Tests;

public static class VerifyConfiguration
{
    [ModuleInitializer]
    public static void Init()
    {
        Verifier.UseProjectRelativeDirectory("Expectations");
    }
}
