using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class AgonesAllocationService
{
    private readonly BackendServerRpcClient _agonesServerRpcClient;
    private readonly AgonesAllocatorApiClient _agonesAllocatorApiClient;
    private readonly IAgonesAllocationDatabase _database;
    private readonly KubernetesApiClient _kubernetesClient;
    private readonly ILogger<AgonesAllocationService> _logger;

    public AgonesAllocationService(IAgonesAllocationDatabase database, BackendServerRpcClient agonesRpcClient, KubernetesApiClient kubernetesClient, AgonesAllocatorApiClient agonesAllocatorApiClient, ILogger<AgonesAllocationService> logger)
    {
        _database = database;
        _agonesServerRpcClient = agonesRpcClient;
        _kubernetesClient = kubernetesClient;
        _agonesAllocatorApiClient = agonesAllocatorApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetKubernetesApiAsync()
    {
        if (KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            return await _kubernetesClient.SendKubernetesApiAsync("/api", HttpMethod.Get);
        }
        else
        {
            return $"Frontend is not running Kubernetes. Use Agones instead of Kubernetes when AgonesServer address is input to 'Input'.";
        }
    }

    /// <summary>
    /// Send Allocation request to Agones through Agones Allocation API.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationApiAsync(string endpoint, string @namespace, string fleetName)
    {
        var body = AgonesAllocationApiRequest.CreateRequest(@namespace, fleetName);
        var response = await _agonesAllocatorApiClient.SendAllocationApiAsync(endpoint, body);

        // debug output response JSON
        _logger.LogDebug(JsonSerializer.Serialize(response));

        var host = response.address;
        var port = response.ports!.First().port;
        AddAllocationEntry($"{host}:{port}");
        return $"http://{host}:{port}";
    }

    /// <summary>
    /// Send Allocation request to Agones through Kubernetes Custom Resource Definition.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationCrdAsync(string @namespace, string fleetName)
    {
        var body = KubernetesAgonesGameServerAllocationRequest.CreateRequest("allocation", fleetName);

        // ref: https://agones.dev/site/docs/guides/access-api/
        var json = JsonSerializer.Serialize(body);
        _logger.LogDebug(json);
        var content = new StringContent(json);
        var response = await _kubernetesClient.SendKubernetesApiAsync<KubernetesAgonesGameServerAllocationResponse>($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", HttpMethod.Post, content);

        if (response == null) throw new ArgumentNullException(nameof(response));

        // debug output response JSON
        _logger.LogDebug(JsonSerializer.Serialize(response));

        var host = response.status!.address;
        var port = response.status!.ports!.First().port;
        AddAllocationEntry($"{host}:{port}");
        return $"http://{host}:{port}";
    }

    /// <summary>
    /// Send Allocation request to Agones through Backend.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationBackendAsync(string backendserverAddress)
    {
        var response = await _agonesServerRpcClient.AllocateCrdAsync(backendserverAddress);

        var uri = new Uri(backendserverAddress);
        var host = uri.Host;
        var port = uri.Port;
        AddAllocationEntry($"{host}:{port}");
        return $"http://{host}:{port}";
    }

    #region AllocationEntry

    public void AddAllocationEntry(string item) => _database.Add(item);
    public void RemoveAllocationEntry(string item) => _database.Remove(item);
    public void ClearAllocationEntries() => _database.Clear();

    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string[] GetAllAllocationEntries()
    {
        if (_database.Count == 0)
            return Array.Empty<string>();

        return _database.Items.Select(x => $"http://{x}").ToArray();
    }
    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string? GetAllocationEntryRandomOrDefault()
    {
        if (_database.Count == 0)
            return null;

        var skip = Random.Shared.Next(0, _database.Count);
        var item = _database.Items.Skip(skip).First();
        return $"http://{item}";
    }
    #endregion

}
