using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Data;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class Allocate : ComponentBase
{
    [Inject]
    public AgonesAllocationSet AgonesAllocationList { get; set; } = default!;
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    public string? Result { get; set; }
    public string[] List { get; set; } = Array.Empty<string>();
    public string Host { get; set; } = "";
    public int Port { get; set; } = 0;

    private async Task AllocateAsync()
    {
        if (Host is not null)
        {
            AgonesAllocationList.Add(new AgonesAllocation
            {
                Host = Host,
                Port = Port,
            });
            Result = AgonesAllocationList.GetAddressRandomOrDefault();
            List = AgonesAllocationList.GetAll();
        }
        else if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            Result = "Frontend is not running Kubernetse. You cannot execute allocate.";
        }
        else
        {
            var endpoint = KubernetesServiceProvider.Current.KubernetesServiceEndPoint;
            var accessToken = KubernetesServiceProvider.Current.AccessToken;

            // todo: Allocation を Post する。
            var httpClient = HttpClientFactory.CreateClient("kubernetes");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}/api");
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            var result = await httpClient.SendAsync(request);
            Result = await result.Content.ReadAsStringAsync();
            List = AgonesAllocationList.GetAll();
        }

        StateHasChanged();
    }

    private Task ClearAsync()
    {
        AgonesAllocationList.Clear();
        Result = null;
        List = Array.Empty<string>();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task ListAsync()
    {
        Result = null;
        List = AgonesAllocationList.GetAll();
        StateHasChanged();
        return Task.CompletedTask;
    }
}
