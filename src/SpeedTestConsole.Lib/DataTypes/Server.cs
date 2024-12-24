namespace SpeedTestConsole.Lib.DataTypes;

public sealed class Server
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string? Name { get; set; }

    [XmlAttribute("country")]
    public string? Country { get; set; }

    [XmlAttribute("sponsor")]
    public string? Sponsor { get; set; }

    [XmlAttribute("host")]
    public string? Host { get; set; }

    [XmlAttribute("url")]
    public string? Url { get; set; }

    [XmlAttribute("lat")]
    public double Latitude { get; set; }

    [XmlAttribute("lon")]
    public double Longitude { get; set; }

    /// <summary>
    /// Latency between client and server
    /// </summary>
    /// <remarks>
    /// Not returned by the Speedtest call to fetch the lest of servers, 
    /// but used by the client to store the latency (once determined)
    /// </remarks>
    [XmlIgnore]
    public int? Latency { get; set; }
}