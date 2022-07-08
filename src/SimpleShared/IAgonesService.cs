using MagicOnion;
using MessagePack;

namespace SimpleShared;

public interface IAgonesService : IService<IAgonesService>
{
    /// <summary>
    /// Allocate GameServer with SDK.
    /// </summary>
    /// <returns></returns>
    UnaryResult<AgonesResult> AllocateAsync();
    /// <summary>
    /// Connect to AgonesSDK
    /// </summary>
    /// <returns></returns>
    UnaryResult<AgonesResult> ConnectAsync();
    /// <summary>
    /// Set GameServer Status to READY
    /// </summary>
    /// <returns></returns>
    UnaryResult<AgonesResult> ReadyAsync();
    /// <summary>
    /// Shutdown GameServer
    /// </summary>
    /// <returns></returns>
    UnaryResult<AgonesResult> ShutdownAsync();
    /// <summary>
    /// Get GameServer Information
    /// </summary>
    /// <returns></returns>
    UnaryResult<AgonesResult> GetGameServerAsync();
}

[MessagePackObject(true)]
public class AgonesResult
{
    public bool Success { get; }
    public string Detail { get; }

    public AgonesResult(bool success, string detail)
    {
        Success = success;
        Detail = detail;
    }
}

