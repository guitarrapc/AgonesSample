using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Data;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class AgonesService : ComponentBase
{
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    private string? _address;
    public string Result { get; set; } = "";
    public string Input { get; set; } = "";

    private async Task AllocateAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.AllocateAsync();
            Result = $"{result.Success} {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            throw;
        }
    }

    private async Task ReadyAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.ReadyAsync();
            Result = $"{result.Success} {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            throw;
        }
    }

    private async Task ShutdownAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.ShutdownAsync();
            Result = $"{result.Success} {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            throw;
        }
    }
}
