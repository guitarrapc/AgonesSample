using MagicOnion;
using MessagePack;

namespace SimpleShared;

public interface IAgonesService : IService<IAgonesService>
{
    UnaryResult<AgonesResult> AllocateAsync();
    UnaryResult<AgonesResult> ReadyAsync();
    UnaryResult<AgonesResult> ShutdownAsync();
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
