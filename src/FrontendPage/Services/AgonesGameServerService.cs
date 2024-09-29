using FrontendPage.Clients;
using FrontendPage.Data;
using FrontendPage.Infrastructures;
using Shared;
using Shared.AgonesCrd;
using System.Text.Json;

namespace FrontendPage.Services;

public class AgonesGameServerService(IAgonesAllocationDatabase database, AgonesMagicOnionClient agonesRpcService, KubernetesApiClient kubernetesApi)
{
    public async Task<(GameServerListCrdResponse? response, string json)> GetGameServersAsync(string @namespace)
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
    /// Send request to Kubernetes API endpoint.
    /// </summary>
    /// <returns></returns>
    private async Task<(GameServerListCrdResponse? response, string json)> GetGameServersKubernetesAsync(string @namespace)
    {
        // ref: https://agones.dev/site/docs/guides/access-api/
        var json = await kubernetesApi.GetGameServersAsync(@namespace);
        var response = JsonSerializer.Deserialize<GameServerListCrdResponse>(json);
        return (response, json);
    }

    /// <summary>
    /// Send request to Agones GameServer API.
    /// </summary>
    /// <returns></returns>
    private async Task<(GameServerListCrdResponse? response, string json)> GetGameServersAgonesAsync()
    {
        var servers = database.Items.Select(x => $"http://{x}").ToArray();
        var tasks = servers.Select(x => agonesRpcService.GetGameServerAsync(x));
        var gameServerResponses = await Task.WhenAll(tasks);

        var items = gameServerResponses
            .Where(x => x.IsSuccess)
            .Select(x => x.Detail)
            .Select(x => JsonSerializer.Deserialize<AgonesSdkGameServerMock>(x))
            .Where(x => x is not null)
            .Select(x => x!.ToAgonesCrdGameServerResponse(nodeName: "localhost"))
            .ToArray()
            ?? [];

        var response = new GameServerListCrdResponse
        {
            items = items,
        };
        var json = JsonSerializer.Serialize(response);
        return (response, json);
    }
}
