using Shared.AgonesCrd;
using System.Reflection;

namespace FrontendPage.Data;

public class AgonesSdkGameServerMock
{
    public Objectmeta? objectMeta { get; set; }
    public Spec? spec { get; set; }
    public Status? status { get; set; }

    public class Objectmeta
    {
        public string? name { get; set; }
        public string? @namespace { get; set; }
        public string? uid { get; set; }
        public string? resourceVersion { get; set; }
        public int generation { get; set; }
        public int creationTimestamp { get; set; }
        public int deletionTimestamp { get; set; }
        public Dictionary<string, string>? annotations { get; set; }
        public Dictionary<string, string>? labels { get; set; }
    }

    public class Spec
    {
        public Health? health { get; set; }
    }

    public class Health
    {
        public bool disabled { get; set; }
        public int periodSeconds { get; set; }
        public int failureThreshold { get; set; }
        public int initialDelaySeconds { get; set; }
    }

    public class Status
    {
        public string? state { get; set; }
        public string? address { get; set; }
        public Adddress[]? addresses { get; set; }
        public Port[]? ports { get; set; }
        public object? players { get; set; }
    }

    public class Adddress
    {
        public string? type { get; set; }
        public string? address { get; set; }
    }

    public class Port
    {
        public string? name { get; set; }
        public int port_ { get; set; }
    }

    public GameServerResponse ToAgonesCrdGameServerResponse(string nodeName = "")
    {
        return new GameServerResponse
        {
            metadata = new GameServerResponse.Metadata
            {
                name = objectMeta?.name,
                @namespace = objectMeta?.@namespace,
                annotations = objectMeta?.annotations,
                labels = objectMeta?.labels,
                creationTimestamp = new FileInfo(Assembly.GetEntryAssembly()!.Location).LastWriteTimeUtc, // It's mock. Let's use Assembly write time.
            },
            spec = new GameServerResponse.Spec
            {
                health = new GameServerResponse.Health
                {
                    failureThreshold = spec?.health?.failureThreshold ?? 0,
                    initialDelaySeconds = spec?.health?.initialDelaySeconds ?? 0,
                    periodSeconds = spec?.health?.periodSeconds ?? 0,
                },
            },
            status = new GameServerResponse.Status
            {
                address = status?.address,
                ports = status?.ports?.Select(x => new GameServerResponse.Port
                {
                    name = x.name,
                    port = x.port_,
                })
                .ToArray(),
                state = status?.state,
                nodeName = nodeName,
            },
        };
    }
}
