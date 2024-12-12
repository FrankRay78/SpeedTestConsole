namespace SpeedTestConsole.Lib.DataTypes;

public class Server
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
}