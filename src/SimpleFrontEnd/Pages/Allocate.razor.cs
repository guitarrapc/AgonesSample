using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Data;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class Allocate : ComponentBase
{
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    public string? Result { get; set; }
    public string[] List { get; set; } = Array.Empty<string>();
    public string Host { get; set; } = "";
    public int Port { get; set; } = 0;

    private async Task TestKubernetesApiAsync()
    {
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            Result = "Frontend is not running Kubernetes. You cannot execute allocate.";
            List = AgonesAllocationService.GetAllAllocationEntries();
        }
        else
        {
            var result = await AgonesAllocationService.GetKubernetesApiAsync();
            Result = result;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }

        StateHasChanged();

    }

    private async Task AllocateAsync()
    {
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            Result = "Frontend is not running Kubernetse. You cannot execute allocate.";
            List = AgonesAllocationService.GetAllAllocationEntries();
        }
        else
        {
            // todo: Allocation を Post する。
            var result = await AgonesAllocationService.GetKubernetesApiAsync();
            Result = result;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }

        StateHasChanged();
    }

    private Task ManualAllocateAsync()
    {
        AgonesAllocationService.AddAllocationEntry(new AgonesAllocation
        {
            Host = Host,
            Port = Port,
        });
        Result = $"http://{Host}:{Port}";
        List = AgonesAllocationService.GetAllAllocationEntries();

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task ClearAsync()
    {
        AgonesAllocationService.ClearAllocationEntries();
        Result = null;
        List = Array.Empty<string>();

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task ListAsync()
    {
        Result = null;
        List = AgonesAllocationService.GetAllAllocationEntries();

        StateHasChanged();
        return Task.CompletedTask;
    }
}
