using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Models;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class AgonesService : ComponentBase
{
    [Inject]
    public Models.BackendServerRpcClient AgonesServerService { get; set; } = default!;
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

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
            var result = await AgonesServerService.AllocateCrdAsync("http://" + Input);
            Result = result.Result;
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace ?? "";
        }
    }

    private async Task ConnectAsync()
    {
        try
        {
            var result = await AgonesServerService.ConnectAsync("http://" + Input);
            Result = result.Result;
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace ?? "";
        }
    }

    private async Task ReadyAsync()
    {
        try
        {
            var result = await AgonesServerService.ReadyAsync("http://" + Input);
            Result = result.Result;
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace ?? "";
        }
    }

    private async Task GetGameServerAsync()
    {
        try
        {
            var result = await AgonesServerService.GetGameServerAsync("http://" + Input);
            Result = result.Result;
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace ?? "";
        }
    }

    private async Task ShutdownAsync()
    {
        try
        {
            var result = await AgonesServerService.ShutdownAsync("http://" + Input);
            AgonesAllocationService.RemoveAllocationEntry(Input);
            Result = result.Result;
            Detail = result.Detail;
        }
        catch (Exception ex)
        {
            Result = ex.Message;
            Detail = ex.StackTrace ?? "";
        }
    }
}
