using System.Text.Json;
using Agones;
using MagicOnion;
using MagicOnion.Server;
using SimpleShared;

namespace SimpleGrpc.Services;

public class AgonesService : ServiceBase<IAgonesService>, IAgonesService
{
    private readonly IAgonesSDK _agonesSdk;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public AgonesService(IAgonesSDK agonesSdk)
    {
        _agonesSdk = agonesSdk;
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public async UnaryResult<AgonesResult> AllocateAsync()
    {
        var res = await _agonesSdk.AllocateAsync();
        return new AgonesResult(res.StatusCode == Grpc.Core.StatusCode.OK, res.Detail);
    }

    public async UnaryResult<AgonesResult> ConnectAsync()
    {
        var success = await _agonesSdk.ConnectAsync();
        return new AgonesResult(success, "");
    }

    public async UnaryResult<AgonesResult> GetGameServerAsync()
    {
        var res = await _agonesSdk.GetGameServerAsync();
        var json = JsonSerializer.Serialize(res, _jsonSerializerOptions).Replace("\r\n", "\n");
        return new AgonesResult(true, json);
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
