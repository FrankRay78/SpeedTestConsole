using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTestConsole.Lib.DataTypes;

/// <summary>
/// Represents a server used for network speed testing.
/// </summary>
/// <remarks>
/// Contains the minimum set of properties to represent a server.
/// </remarks>
public interface IServer
{
    public string? Name { get; set; }
    public string? Sponsor { get; set; }
    public string? Url { get; set; }
}