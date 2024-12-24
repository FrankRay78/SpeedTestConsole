namespace SpeedTestConsole.Lib.Extensions;

public static class ServerExtensions
{
    public static Server? GetFastestServerByLatency(this Server[] servers)
    {
        return servers.Where(x => x.Latency.HasValue).OrderBy(x => x.Latency).FirstOrDefault();
    }
}