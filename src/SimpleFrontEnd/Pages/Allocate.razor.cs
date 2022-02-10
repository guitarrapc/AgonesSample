using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Models;
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
    public string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? DockerServiceProvider.Current.IsRunningOnDocker
            ? "server:80" // docker
            : "localhost:5157" // local machine
        : ""; // kubernetes

    private async Task TestKubernetesApiAsync()
    {
        var apiResult = await AgonesAllocationService.GetKubernetesApiAsync();
        Result = apiResult;

        StateHasChanged();

    }

    private async Task AllocateAsync()
    {
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            if (string.IsNullOrEmpty(Input))
            {
                Result = "Frontend is not running Kubernetse. Use Agones instead of Kubernetes when AgonesServer address is input.";
                List = AgonesAllocationService.GetAllAllocationEntries();
                return;
            }

            try
            {
                var host = await AgonesAllocationService.SendAllocationAgonesAsync($"http://{Input}");
                Result = host;
                List = AgonesAllocationService.GetAllAllocationEntries();
            }
            catch (Exception ex)
            {
                Result = ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
                List = Array.Empty<string>();
            }
        }
        else
        {
            var host = await AgonesAllocationService.SendAllocationAsync("default", "simple-server");
            Result = host;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }

        StateHasChanged();
    }

    private Task ManualAllocateAsync()
    {
        AgonesAllocationService.AddAllocationEntry(Input);
        Result = $"http://{Input}";
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
