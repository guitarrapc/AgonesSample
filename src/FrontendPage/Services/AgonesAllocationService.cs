using FrontendPage.Clients;
using FrontendPage.Infrastructures;
using Shared;
using Shared.AgonesApi;
using Shared.AgonesCrd;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrontendPage.Services;

public class AgonesAllocationService(IAgonesAllocationDatabase database, AgonesMagicOnionClient agonesRpcClient, KubernetesApiClient kubernetesClient, AgonesApiClient agonesAllocatorApiClient, ILogger<AgonesAllocationService> logger)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetKubernetesApiAsync()
    {
        if (KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            return await kubernetesClient.GetKubernetesApiRoot();
        }
        else
        {
            return $"Not running on Kubernetes. Use Agones API instead of Kubernetes API.";
        }
    }

    /// <summary>
    /// Send Allocation request to Agones through Agones Allocation API.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationApiAsync(string endpoint, string @namespace, string fleetName)
    {
        var body = GameServerAllocationApiRequest.CreateRequest(@namespace, fleetName);
        var json = await agonesAllocatorApiClient.AllocateAsync(endpoint, body);

        logger.LogDebug(json);

        var allocationResult = JsonSerializer.Deserialize<GameServerAgonesAllocationApiResponse>(json, _jsonSerializerOptions);
        var host = allocationResult!.address;
        var port = allocationResult!.ports!.First().port;

        AddAllocationEntry($"{host}:{port}");
        return ($"http://{host}:{port}", json);
    }

    /// <summary>
    /// Send Allocation request to Agones through Kubernetes Custom Resource Definition.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationCrdAsync(string @namespace, string fleetName)
    {
        var body = GameServerAllocationCrdRequest.CreateRequest("allocation", fleetName);
        var json = await kubernetesClient.AllocateAsync(@namespace, body);

        logger.LogDebug(json);

        var allicationResult = JsonSerializer.Deserialize<GameServerAllocationCrdResponse>(json, _jsonSerializerOptions);
        var host = allicationResult!.status!.address;
        var port = allicationResult!.status!.ports!.First().port;

        AddAllocationEntry($"{host}:{port}");
        return ($"http://{host}:{port}", json);
    }

    /// <summary>
    /// Send Allocation request to Agones through Backend.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationBackendAsync(string backendserverAddress)
    {
        var response = await agonesRpcClient.AllocateAsync(backendserverAddress);

        var responseJson = JsonSerializer.Serialize(response, _jsonSerializerOptions);
        logger.LogDebug(responseJson);

        var uri = new Uri(backendserverAddress);
        var host = uri.Host;
        var port = uri.Port;

        AddAllocationEntry($"{host}:{port}");
        return ($"http://{host}:{port}", responseJson);
    }

    #region AllocationEntry

    public void AddAllocationEntry(string item) => database.Add(item);
    public void RemoveAllocationEntry(string item) => database.Remove(item);
    public void ClearAllocationEntries() => database.Clear();

    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string[] GetAllAllocationEntries()
    {
        if (database.Count == 0)
            return Array.Empty<string>();

        return database.Items.Select(x => $"http://{x}").ToArray();
    }
    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string? GetAllocationEntryRandomOrDefault()
    {
        if (database.Count == 0)
            return null;

        var skip = Random.Shared.Next(0, database.Count);
        var item = database.Items.Skip(skip).First();
        return $"http://{item}";
    }
    #endregion

}
