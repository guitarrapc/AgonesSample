using Agones;
using MagicOnion;
using MagicOnion.Server;
using SimpleShared;

namespace SimpleGrpc.Services;

public class AgonesService : ServiceBase<IAgonesService>, IAgonesService
{
    private readonly AgonesSDK _agonesSdk;

    public AgonesService(AgonesSDK agonesSdk)
    {
        _agonesSdk = agonesSdk;
    }

    public async UnaryResult<AgonesResult> AllocateAsync()
    {
        var res = await _agonesSdk.AllocateAsync();
        return new AgonesResult(res.StatusCode == Grpc.Core.StatusCode.OK, res.Detail);
    }

    public async UnaryResult<AgonesResult> ReadyAsync()
    {
        var res = await _agonesSdk.ReadyAsync();
        return new AgonesResult(res.StatusCode == Grpc.Core.StatusCode.OK, res.Detail);
    }

    public async UnaryResult<AgonesResult> ShutdownAsync()
    {
        var res = await _agonesSdk.ShutDownAsync();
        return new AgonesResult(res.StatusCode == Grpc.Core.StatusCode.OK, res.Detail);
    }
}
