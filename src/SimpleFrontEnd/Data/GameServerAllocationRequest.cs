using System.Text.Json.Serialization;

namespace SimpleFrontEnd.Data;

// ref: https://agones.dev/site/docs/reference/gameserverallocation/
// '{"apiVersion":"allocation.agones.dev/v1","kind":"GameServerAllocation","spec":{"selectors":["matchLabels":{"agones.dev/fleet":"FLEETNAME"}]}}'
public abstract class GameServerAllocationBase
{
    public string apiVersion { get; set; } = "allocation.agones.dev/v1";
    public string kind { get; set; } = "GameServerAllocation";
    public Spec? spec { get; set; }
    public Metadata? metadata { get; set; }

    public class Spec
    {
        public Selector[]? selectors { get; set; }
        public string? scheduling { get; set; } = "Packed"; // Packed, Distributed
        public Metadata? metadata { get; set; }
    }

    public class Metadata
    {
        public string? name { get; set; }
        public Dictionary<string, string>? labels { get; set; }
        public Dictionary<string, string>? annotations { get; set; }
    }

    public class Selector
    {
        public Dictionary<string, string>? matchLabels { get; set; }
        public Players? players { get; set; }
        public Matchexpression[]? matchExpressions { get; set; }
        public string gameServerState { get; set; } = "Ready"; // Ready, Allocated
    }

    public class Players
    {
        public int minAvailable { get; set; }
        public int maxAvailable { get; set; }
    }

    public class Matchexpression
    {
        public string? key { get; set; }
        [JsonPropertyName("operator")]
        public string? Operator { get; set; }
        public string[]? values { get; set; }
    }
}


public class GameServerAllocationRequest : GameServerAllocationBase
{
    public static GameServerAllocationRequest CreateRequest(string name, string fleetName)
    {
        var request = new GameServerAllocationRequest()
        {
            metadata = new Metadata
            {
                name = name,
                annotations = new Dictionary<string, string> { },
                labels = new Dictionary<string, string> { },
            },
            spec = new Spec
            {
                selectors = new Selector[]
                {
                    new Selector
                    {
                        matchLabels = new Dictionary<string, string>
                        {
                            { "agones.dev/fleet", fleetName }
                        },
                        gameServerState = "Ready",
                    }
                }
            }
        };
        return request;
    }
}


public class GameServerAllocationResponse : GameServerAllocationBase
{
    public Status? status { get; set; }
    public class Status
    {
        public string state { get; set; }
        public string gameServerName { get; set; }
        public Port[] ports { get; set; }
        public string address { get; set; }
        public string nodeName { get; set; }
    }

    public class Port
    {
        public string name { get; set; }
        public int port { get; set; }
    }
}
