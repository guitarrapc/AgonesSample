using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Models;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class AgonesService : ComponentBase
{
    [Inject]
    public Models.BackendServerRpcClient AgonesServerService { get; set; } = default!;

    private string _address = "";
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
            var result = await AgonesServerService.AllocateCrdAsync(_address);
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
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            var result = await AgonesServerService.ConnectAsync(_address);
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
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            var result = await AgonesServerService.ReadyAsync(_address);
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
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            var result = await AgonesServerService.GetGameServerAsync(_address);
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
            _address = Input.StartsWith("http://") ? Input : $"http://{Input}";
            var result = await AgonesServerService.ShutdownAsync(_address);
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
