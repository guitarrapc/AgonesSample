namespace Shared.AgonesCrd;

// detail: https://agones.dev/site/docs/advanced/allocator-service/#api-reference
// see: Swagger API definition. https://github.com/googleforgames/agones/blob/release-1.24.0/pkg/allocation/go/allocation.swagger.json

public class AllocationRequest
{
    public static AllocationRequest CreateRequest(string @namespace, string fleetName)
    {
        return new AllocationRequest
        {
            @namespace = @namespace,
            gameServerSelectors = new[]
            {
                new Gameserverselector
                {
                    matchLabels = new Dictionary<string, string>
                    {
                        { "agones.dev/fleet", fleetName }
                    },
                    gameServerState = "READY",
                }
            }
        };
    }

    public string? @namespace { get; init; }
    public Multiclustersetting? multiClusterSetting { get; init; }
    public Gameserverselector? requiredGameServerSelector { get; init; }
    public Gameserverselector[]? preferredGameServerSelectors { get; init; }
    /// <summary>
    /// Packed or Distributed
    /// </summary>
    public string? scheduling { get; init; }
    public Metapatch? metaPatch { get; init; }
    public Metadata? metadata { get; init; }
    public Gameserverselector[]? gameServerSelectors { get; init; }

    public class Multiclustersetting
    {
        public bool enabled { get; init; }
        public Policyselector? policySelector { get; init; }
    }

    public class Policyselector
    {
        public Dictionary<string, string>? matchLabels { get; init; }
    }

    public class Gameserverselector
    {
        public Dictionary<string, string>? matchLabels { get; init; }
        /// <summary>
        /// READY or ALLOCATED
        /// </summary>
        public string? gameServerState { get; init; }
        public Players? players { get; init; }
    }

    public class Players
    {
        public string? minAvailable { get; init; }
        public string? maxAvailable { get; init; }
    }

    public class Metapatch
    {
        public Dictionary<string, string>? labels { get; init; }
        public Dictionary<string, string>? annotations { get; init; }
    }

    public class Metadata
    {
        public Dictionary<string, string>? labels { get; init; }
        public Dictionary<string, string>? annotations { get; init; }
    }
}

public class AllocationResponse
{
    public string? gameServerName { get; init; }
    public PortType[]? ports { get; init; }
    public string? address { get; init; }
    public string? nodeName { get; init; }

    // for error response
    public string? error { get; set; }
    public int code { get; set; }
    public string? message { get; set; }

    public record PortType
    {
        public string? name { get; init; }
        public int port { get; init; }
    }
}
