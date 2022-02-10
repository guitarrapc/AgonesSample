using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class AgonesAllocationService
{
    private readonly ILogger<AgonesAllocationService> _logger;
    private readonly KubernetesApiService _kubernetesApi;
    private readonly IAgonesAllocationDatabase _database;
    private readonly AgonesServerRpcService _agonesServerRpcService;

    public AgonesAllocationService(IAgonesAllocationDatabase database, AgonesServerRpcService agonesRpcService, KubernetesApiService kubernetesApi, ILogger<AgonesAllocationService> logger)
    {
        _database = database;
        _agonesServerRpcService = agonesRpcService;
        _kubernetesApi = kubernetesApi;
        _logger = logger;
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetKubernetesApiAsync()
    {
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            return $"Frontend is not running Kubernetes. Use Agones instead of Kubernetes when AgonesServer address is input to 'Input'.";
        }
        else
        {
            return await _kubernetesApi.SendKubernetesApiAsync("/api", HttpMethod.Get);
        }
    }

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationAsync(string @namespace, string fleetName)
    {
        var body = KubernetesAgonesGameServerAllocationRequest.CreateRequest("allocation", fleetName);
        var json = JsonSerializer.Serialize(body);
        _logger.LogDebug(json);

        // ref: https://agones.dev/site/docs/guides/access-api/
        var content = new StringContent(json);
        var response = await _kubernetesApi.SendKubernetesApiAsync<KubernetesAgonesGameServerAllocationResponse>($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", HttpMethod.Post, content);

        if (response == null) throw new ArgumentNullException(nameof(response));

        // debug output response JSON
        _logger.LogDebug(JsonSerializer.Serialize(response));

        var host = response.status!.address;
        var port = response.status!.ports!.First().port;
        AddAllocationEntry($"{host}:{port}");
        return $"http://{host}:{port}";
    }

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationAgonesAsync(string address)
    {
        var response = await _agonesServerRpcService.AllocateAsync(address);

        var uri = new Uri(address);
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
