using FrontendPage.Services;
using Microsoft.AspNetCore.Components;
using Shared;

namespace FrontendPage.Components.Pages;

public partial class Allocate : ComponentBase
{
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    private string? Result { get; set; }
    public string Message { get; set; } = "";
    public string Detail { get; set; } = "";
    private string[] List { get; set; } = [];
    private string Input { get; set; } = !KubernetesServiceProvider.Current.IsRunningOnKubernetes
        ? KubernetesServiceProvider.Current.IsRunningOnDocker
            ? "server:5157" // docker
            : "localhost:5157" // local machine
        : "agones-allocator.agones-system.svc.cluster.local:8443"; // kubernetes
    private const string Namespace = "default";
    private const string FleetName = "simple-backend";

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
            var (endpoint, json) = await AgonesAllocationService.SendAllocationApiAsync($"http://{Input}", Namespace, FleetName);
            List = AgonesAllocationService.GetAllAllocationEntries();
            Result = endpoint;
            Message = json;
        }
        catch (Exception ex)
        {
            List = Array.Empty<string>();
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            Message = ex.Message;
            Detail = ex.StackTrace!;
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
            var (endpoint, json) = await AgonesAllocationService.SendAllocationBackendAsync($"http://{Input}");
            Result = endpoint;
            List = AgonesAllocationService.GetAllAllocationEntries();
            Result = endpoint;
            Message = json;
        }
        catch (Exception ex)
        {
            List = Array.Empty<string>();
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            Message = ex.Message;
            Detail = ex.StackTrace!;
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
            var (endpoint, json) = await AgonesAllocationService.SendAllocationCrdAsync(Namespace, FleetName);
            Result = endpoint;
            List = AgonesAllocationService.GetAllAllocationEntries();
            Result = endpoint;
            Message = json;
        }
        catch (Exception ex)
        {
            List = Array.Empty<string>();
            Result = ex.GetType().FullName + " " + ex.Message + $"{(ex.StackTrace != null ? " " + ex.StackTrace : "")}";
            Message = ex.Message;
            Detail = ex.StackTrace!;
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
