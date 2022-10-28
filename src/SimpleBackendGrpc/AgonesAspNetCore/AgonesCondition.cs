namespace AgonesAspNetCore;

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

/// <summary>
/// Indicate current Agones Condition
/// </summary>
public class AgonesCondition
{
    /// <summary>
    /// When AgonesSdk is exists, status should be true.
    /// </summary>
    public bool IsConnected => _isConnected;
    private bool _isConnected;
    /// <summary>
    /// Healthy status should keep true while running as Agones GameServer.
    /// </summary>
    public bool HealthStatus => _healthStatus;
    private bool _healthStatus;
    /// <summary>
    /// Indicate Agones State of lifecycle. Should be match to GameServer Status.
    /// </summary>
    public AgonesState State => _state;
    private AgonesState _state;
    /// <summary>
    /// Last Update Datetime (UTC)
    /// </summary>
    public DateTime LastUpdate => _lastUpdate;
    private DateTime _lastUpdate;

    public AgonesCondition()
    {
        _isConnected = false;
        _healthStatus = false;
        _state = AgonesState.Scheduled;
        _lastUpdate = DateTime.UtcNow;
    }

    public void Connected(bool conected) => (_isConnected, _lastUpdate) = (conected, DateTime.UtcNow);
    public void Healthy(Grpc.Core.StatusCode statusCode) => (_healthStatus, _lastUpdate) = (statusCode == Grpc.Core.StatusCode.OK, DateTime.UtcNow);
    public void UpdateState(AgonesState state) => (_state, _lastUpdate) = (state, DateTime.UtcNow);
}
