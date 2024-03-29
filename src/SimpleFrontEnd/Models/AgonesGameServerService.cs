﻿using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class AgonesGameServerService
{
    private readonly IAgonesAllocationDatabase _database;
    private readonly ILogger<AgonesGameServerService> _logger;
    private readonly BackendServerRpcClient _agonesServerRpcService;
    private readonly KubernetesApiClient _kubernetesApi;

    public AgonesGameServerService(IAgonesAllocationDatabase database, BackendServerRpcClient agonesRpcService, KubernetesApiClient kubernetesApi, ILogger<AgonesGameServerService> logger)
    {
        _database = database;
        _agonesServerRpcService = agonesRpcService;
        _kubernetesApi = kubernetesApi;
        _logger = logger;
    }

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
        var json = await _kubernetesApi.SendKubernetesApiAsync($"/apis/agones.dev/v1/namespaces/{@namespace}/gameservers", HttpMethod.Get);
        var response = JsonSerializer.Deserialize<KubernetesAgonesGameServerListResponse>(json);
        return response;
    }

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    private async Task<KubernetesAgonesGameServerListResponse?> GetGameServersAgonesAsync()
    {
        var servers = _database.Items.Select(x => $"http://{x}").ToArray();
        var tasks = servers.Select(x => _agonesServerRpcService.GetGameServerAsync(x));
        var gameServerResponses = await Task.WhenAll(tasks);

        var items = gameServerResponses.Select(x => x.Detail)
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
