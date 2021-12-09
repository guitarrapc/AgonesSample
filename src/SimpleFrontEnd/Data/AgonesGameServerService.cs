using System.Text.Json;

namespace SimpleFrontEnd.Data;

public class AgonesGameServerService
{
    private readonly ILogger<AgonesGameServerService> _logger;
    private readonly KubernetesApiService _kubernetesApi;

    public AgonesGameServerService(KubernetesApiService kubernetesApi, ILogger<AgonesGameServerService> logger)
    {
        _logger = logger;
        _kubernetesApi = kubernetesApi;
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public async Task<GameServerListResponse?> GetGameServersAsync(string @namespace)
    {
        // ref: https://agones.dev/site/docs/guides/access-api/
        var json = await _kubernetesApi.SendKubernetesApiAsync($"/apis/agones.dev/v1/namespaces/{@namespace}/gameservers", HttpMethod.Get);
        var response = JsonSerializer.Deserialize<GameServerListResponse>(json);
        return response;
    }
}
