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
    public string Detail { get; set; } = "";
    public string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? DockerServiceProvider.Current.IsRunningOnDocker
            ? "server:80" // docker
            : "localhost:5157" // local machine
        : ""; // kubernetes

    private async Task AllocateAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.AllocateAsync();
            Result = $"{(result.Success ? "Success" : "Failed")} {result.Detail}";
            Detail = $"Status change to Allocated. {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace;
        }
    }

    private async Task ConnectAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.ConnectAsync();
            Result = $"{(result.Success ? "Success" : "Failed")} {result.Detail}";
            Detail = $"Connected to AgonesSdk. {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace;
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
            Result = $"{(result.Success ? "Success" : "Failed")}";
            Detail = $"Status change to Ready. {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace;
        }
    }

    private async Task GetGameServerAsync()
    {
        try
        {
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            using var channel = GrpcChannel.ForAddress(_address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.GetGameServerAsync();
            Result = $"{(result.Success ? "Success" : "Failed")}";
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace;
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
            Result = $"{(result.Success ? "Success" : "Failed")} {result.Detail}";
            Detail = $"Status change to Shutdown. {result.Detail}";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace;
        }
    }
}
