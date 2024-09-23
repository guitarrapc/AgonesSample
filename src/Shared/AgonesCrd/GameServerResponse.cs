namespace Shared.AgonesCrd;

/// <summary>
/// Agones GameServer Response from Kubernetes.
/// ref: https://agones.dev/site/docs/reference/gameserver/
/// </summary>
public class GameServerResponse
{
    public string apiVersion { get; set; } = "agones.dev/v1";
    public string kind { get; set; } = "GameServer";
    public Metadata? metadata { get; set; }
    public Spec? spec { get; set; }
    public Status? status { get; set; }

    public class Metadata
    {
        public Dictionary<string, string>? labels { get; set; }
        public Dictionary<string, string>? annotations { get; set; }
        public string? clusterName { get; set; }
        public DateTime creationTimestamp { get; set; }
        public string[]? finalizers { get; set; }
        public int generation { get; set; }
        public string? name { get; set; }
        public string? @namespace { get; set; }
        public string? resourceVersion { get; set; }
        public string? selfLink { get; set; }
        public string? uid { get; set; }
    }

    public class Annotations
    {
        public string? kubectlkubernetesiolastappliedconfiguration { get; set; }
    }

    public class Spec
    {
        public string? PortPolicy { get; set; }
        public string? container { get; set; }
        public int containerPort { get; set; }
        public Health? health { get; set; }
        public int hostPort { get; set; }
        public string? protocol { get; set; }
        public Template? template { get; set; }
    }

    public class Health
    {
        public int failureThreshold { get; set; }
        public int initialDelaySeconds { get; set; }
        public int periodSeconds { get; set; }
    }

    public class Template
    {
        public Metadata1? metadata { get; set; }
        public TemplateSpec? spec { get; set; }
    }

    public class Metadata1
    {
        public object? creationTimestamp { get; set; }
    }

    public class TemplateSpec
    {
        public Container[]? containers { get; set; }
    }

    public class Container
    {
        public string? image { get; set; }
        public string? name { get; set; }
        public Resources? resources { get; set; }
    }

    public class Resources
    {
    }

    public class Status
    {
        public string? address { get; set; }
        public string? nodeName { get; set; }
        public Port[]? ports { get; set; }
        public string? state { get; set; }
    }

    public class Port
    {
        public string? name { get; set; }
        public int port { get; set; }
    }
}
