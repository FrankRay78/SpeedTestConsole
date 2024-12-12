namespace SpeedTestConsole.Lib.DataTypes;

[XmlRoot("settings")]
public class ServersList
{
    [XmlArray("servers")]
    [XmlArrayItem("server")]
    public Server[]? Servers { get; set; }
}