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
    public bool IsSuccess { get; }
    public string Detail { get; }

    public AgonesResult(bool isSuccess, string detail)
    {
        IsSuccess = isSuccess;
        Detail = detail;
    }
}

