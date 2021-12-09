using System.Text.Json;

namespace SimpleFrontEnd.Data;

public class AgonesAllocationService
{
    private readonly ILogger<AgonesAllocationService> _logger;
    private readonly KubernetesApiService _kubernetesApi;
    private readonly IAgonesAllocationDatabase _database;

    public AgonesAllocationService(IAgonesAllocationDatabase database, KubernetesApiService kubernetesApi, ILogger<AgonesAllocationService> logger)
    {
        _logger = logger;
        _kubernetesApi = kubernetesApi;
        _database = database;
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetKubernetesApiAsync() => _kubernetesApi.SendKubernetesApiAsync("/api", HttpMethod.Get);

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationAsync(string @namespace, string fleetName)
    {
        var body = GameServerAllocationRequest.CreateRequest("allocation", fleetName);
        var json = JsonSerializer.Serialize(body);
        // Console.WriteLine(json);

        // ref: https://agones.dev/site/docs/guides/access-api/
        var content = new StringContent(json);
        var response = await _kubernetesApi.SendKubernetesApiAsync<GameServerAllocationResponse>($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", HttpMethod.Post, content);

        var host = response.status!.address;
        var port = response.status!.ports!.First().port;
        AddAllocationEntry(new AgonesAllocation
        {
            Host = host,
            Port = port,
        });
        return $"http://{host}:{port}";
    }

    #region AllocationEntry

    public void AddAllocationEntry(string item) => _database.Add(item);
    public void AddAllocationEntry(AgonesAllocation item) => _database.Add($"{item.Host}:{item.Port}");
    public void RemoveAllocationEntry(string item) => _database.Remove(item);
    public void RemoveAllocationEntry(AgonesAllocation item) => _database.Remove($"{item.Host}:{item.Port}");
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
