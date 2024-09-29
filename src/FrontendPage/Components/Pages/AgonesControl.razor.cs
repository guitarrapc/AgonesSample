using FrontendPage.Clients;
using FrontendPage.Services;
using Microsoft.AspNetCore.Components;
using Shared;

namespace FrontendPage.Components.Pages;

public partial class AgonesControl : ComponentBase
{
    [Inject]
    public AgonesMagicOnionClient AgonesServerService { get; set; } = default!;
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    public string Result { get; set; } = "";
    public string Message { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? KubernetesServiceProvider.Current.IsRunningOnDocker
            ? "server:5157" // docker
            : "localhost:5157" // local machine
        : ""; // kubernetes

    private async Task AllocateAsync()
    {
        var result = await AgonesServerService.AllocateAsync("http://" + Input.Trim());
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
