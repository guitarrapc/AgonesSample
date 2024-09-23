namespace Shared.AgonesCrd;

/// <summary>
/// Agones GameServers List Response from Kubernetes.
/// ref: https://agones.dev/site/docs/reference/gameserver/
/// </summary>
public class GameServerListResponse
{
    public string apiVersion { get; set; } = "agones.dev/v1";
    public GameServerResponse[]? items { get; set; }
    public string kind { get; set; } = "GameServerList";
    public Metadata? metadata { get; set; }

    public class Metadata
    {
        public string? _continue { get; set; }
        public string? resourceVersion { get; set; }
        public string? selfLink { get; set; }
    }
}
