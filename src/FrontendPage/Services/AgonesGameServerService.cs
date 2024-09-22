using FrontendPage.Data;
using FrontendPage.Infrastructures;
using Shared;
using System.Text.Json;

namespace FrontendPage.Services;

public class AgonesGameServerService(IAgonesAllocationDatabase database, AgonesServiceRpcClient agonesRpcService, KubernetesApiClient kubernetesApi)
{
    public async Task<KubernetesAgonesGameServerListResponse?> GetGameServersAsync(string @namespace)
    {
        if (KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            return await GetGameServersKubernetesAsync(@namespace);
        }
        else
        {
            return await GetGameServersAgonesAsync();
        }
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    private async Task<KubernetesAgonesGameServerListResponse?> GetGameServersKubernetesAsync(string @namespace)
    {
        // ref: https://agones.dev/site/docs/guides/access-api/
        var json = await kubernetesApi.SendKubernetesApiAsync($"/apis/agones.dev/v1/namespaces/{@namespace}/gameservers", HttpMethod.Get);
        var response = JsonSerializer.Deserialize<KubernetesAgonesGameServerListResponse>(json);
        return response;
    }

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    private async Task<KubernetesAgonesGameServerListResponse?> GetGameServersAgonesAsync()
    {
        var servers = database.Items.Select(x => $"http://{x}").ToArray();
        var tasks = servers.Select(x => agonesRpcService.GetGameServerAsync(x));
        var gameServerResponses = await Task.WhenAll(tasks);

        var items = gameServerResponses
            .Where(x => x.IsSuccess)
            .Select(x => x.Detail)
            .Select(x => JsonSerializer.Deserialize<AgonesSdkGameServerMock>(x))
            .Where(x => x is not null)
            .Select(x => x!.ToKubernetesAgonesGameServerResponse(nodeName: "localhost"))
            .ToArray()
            ?? Array.Empty<KubernetesAgonesGameServerResponse>();

        return new KubernetesAgonesGameServerListResponse
        {
            items = items,
        };
    }

}
