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

    private string? Result { get; set; }
    private string[] List { get; set; } = Array.Empty<string>();
    private string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? DockerServiceProvider.Current.IsRunningOnDocker
            ? "server:80" // docker
            : "localhost:5157" // local machine
        : ""; // kubernetes
    private const string Namespace = "default";
    private const string FleetName = "simple-server";

    private async Task GetKubernetesApiAsync()
    {
        var apiResult = await AgonesAllocationService.GetKubernetesApiAsync();
        Result = apiResult;

        StateHasChanged();
    }

    private async Task AllocationApiAsync()
    {
        try
        {
            var host = await AgonesAllocationService.SendAllocationApiAsync($"http://{Input}", Namespace, FleetName);
            Result = host;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }
        catch (Exception ex)
        {
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            List = Array.Empty<string>();
        }

        StateHasChanged();
    }

    private async Task AllocationBackendAsync()
    {
        if (string.IsNullOrEmpty(Input))
        {
            Result = "Frontend is not running Kubernetse. Use Agones instead of Kubernetes when AgonesServer address is input.";
            List = AgonesAllocationService.GetAllAllocationEntries();
            return;
        }

        try
        {
            var host = await AgonesAllocationService.SendAllocationBackendAsync($"http://{Input}");
            Result = host;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }
        catch (Exception ex)
        {
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            List = Array.Empty<string>();
        }

        StateHasChanged();
    }

    private async Task AllocationCrdAsync()
    {
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            Result = "Not running on Kubernetes. Allocate via CRD is not permitted.";
            List = Array.Empty<string>();
            StateHasChanged();
            return;
        }

        try
        {
            var host = await AgonesAllocationService.SendAllocationCrdAsync(Namespace, FleetName);
            Result = host;
            List = AgonesAllocationService.GetAllAllocationEntries();
        }
        catch (Exception ex)
        {
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            List = Array.Empty<string>();
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
