using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Infrastructures;
using SimpleFrontEnd.Services;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class AgonesService : ComponentBase
{
    [Inject]
    public AgonesServiceRpcClient AgonesServerService { get; set; } = default!;
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    public string Result { get; set; } = "";
    public string Message { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? DockerServiceProvider.Current.IsRunningOnDocker
            ? "server:80" // docker
            : "localhost:5157" // local machine
        : ""; // kubernetes

    private async Task AllocateAsync()
    {
        var result = await AgonesServerService.AllocateCrdAsync("http://" + Input.Trim());
        Result = result.Status.ToString();
        Message = result.Message;
        Detail = result.Detail;
    }

    private async Task ReadyAsync()
    {
        var result = await AgonesServerService.ReadyAsync("http://" + Input.Trim());
        Result = result.Status.ToString();
        Message = result.Message;
        Detail = result.Detail;
    }

    private async Task GetGameServerAsync()
    {
        var result = await AgonesServerService.GetGameServerAsync("http://" + Input.Trim());
        Result = result.Status.ToString();
        Message = result.Message;
        Detail = result.Detail;
    }

    private async Task ShutdownAsync()
    {
        var result = await AgonesServerService.ShutdownAsync("http://" + Input.Trim());
        Result = result.Status.ToString();
        Message = result.Message;
        Detail = result.Detail;

        if (result.IsSuccess)
        {
            AgonesAllocationService.RemoveAllocationEntry(Input);
        }
    }
}
