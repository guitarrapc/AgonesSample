using Grpc.Net.Client;
using MagicOnion.Client;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class AgonesSdkServiceResponse
{
    public string Result { get; set; } = default!;
    public string Detail { get; set; } = default!;
}

public class BackendServerRpcClient
{
    private readonly ILogger<BackendServerRpcClient> _logger;

    public BackendServerRpcClient(ILogger<BackendServerRpcClient> logger)
    {
        _logger = logger;
    }

    public async Task<AgonesSdkServiceResponse> AllocateCrdAsync(string address)
    {
        _logger.LogInformation($"Sending {AllocateCrdAsync} to {address}.");
        using var channel = GrpcChannel.ForAddress(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        var result = await service.AllocateAsync();
        return new AgonesSdkServiceResponse
        {
            Result = $"{(result.Success ? "Success" : "Failed")} {result.Detail}",
            Detail = $"Status change to Allocated. {result.Detail}",
        };
    }

    public async Task<AgonesSdkServiceResponse> ConnectAsync(string address)
    {
        _logger.LogInformation($"Sending {ConnectAsync} to {address}.");
        using var channel = GrpcChannel.ForAddress(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        var result = await service.ConnectAsync();
        return new AgonesSdkServiceResponse
        {
            Result = result.Success ? "Success" : "Failed",
            Detail = $"Connected to AgonesSdk. {result.Detail}",
        };
    }

    public async Task<AgonesSdkServiceResponse> ReadyAsync(string address)
    {
        _logger.LogInformation($"Sending {ReadyAsync} to {address}.");
        using var channel = GrpcChannel.ForAddress(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        var result = await service.ReadyAsync();
        return new AgonesSdkServiceResponse
        {
            Result = result.Success ? "Success" : "Failed",
            Detail = $"Status change to Ready. {result.Detail}",
        };
    }

    public async Task<AgonesSdkServiceResponse> GetGameServerAsync(string address)
    {
        _logger.LogInformation($"Sending {GetGameServerAsync} to {address}.");
        using var channel = GrpcChannel.ForAddress(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        var result = await service.GetGameServerAsync();
        return new AgonesSdkServiceResponse
        {
            Result = result.Success ? "Success" : "Failed",
            Detail = result.Detail,
        };
    }

    public async Task<AgonesSdkServiceResponse> ShutdownAsync(string address)
    {
        _logger.LogInformation($"Sending {ShutdownAsync} to {address}.");
        using var channel = GrpcChannel.ForAddress(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        var result = await service.ShutdownAsync();

        return new AgonesSdkServiceResponse
        {
            Result = result.Success ? "Success" : "Failed",
            Detail = $"Status change to Shutdown. {result.Detail}",
        };
    }
}
