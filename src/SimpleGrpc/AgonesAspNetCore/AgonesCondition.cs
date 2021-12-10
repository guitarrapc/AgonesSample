namespace SimpleGrpc.AgonesAspNetCore;

/// <summary>
/// Agones GameServer State.
/// </summary>
/// <remarks>https://agones.dev/site/docs/reference/gameserver/</remarks>
public enum AgonesState
{
    Scheduled,
    Ready,
    Reserved,
    Allocated,
    Unhealthy,
    Shutdown,
}

public class AgonesCondition
{
    /// <summary>
    /// Indicate Emulating AgonesSdk or not. Should be true on non-kubernetes environment.
    /// </summary>
    public bool IsEmulating { get; set; }
    /// <summary>
    /// When AgonesSdk is exists, status should be true.
    /// </summary>
    public bool IsConnected { get; set; }
    /// <summary>
    /// Healthy status should keep true while running as Agones GameServer.
    /// </summary>
    public bool HealthStatus { get; set; }
    /// <summary>
    /// Indicate Agones State of lifecycle. Should be match to GameServer Status.
    /// </summary>
    public AgonesState State { get; set; }
}
