using FrontendPage.Infrastructures;
using Shared;
using Shared.AgonesCrd;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrontendPage.Services;

public class AgonesAllocationService(IAgonesAllocationDatabase database, AgonesServiceRpcClient agonesRpcClient, KubernetesApiClient kubernetesClient, AgonesAllocatorApiClient agonesAllocatorApiClient, ILogger<AgonesAllocationService> logger)
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
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
            return await kubernetesClient.SendKubernetesApiAsync("/api", HttpMethod.Get);
        }
        else
        {
            return $"FrontendPage is not running Kubernetes. Use Agones instead of Kubernetes when AgonesServer address is input to 'Input'.";
        }
    }

    /// <summary>
    /// Send Allocation request to Agones through Agones Allocation API.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationApiAsync(string endpoint, string @namespace, string fleetName)
    {
        var body = AllocationRequest.CreateRequest(@namespace, fleetName);
        var response = await agonesAllocatorApiClient.SendAllocationApiAsync(endpoint, body);

        // debug output response JSON
        var responseJson = JsonSerializer.Serialize(response, jsonSerializerOptions);
        logger.LogDebug(responseJson);

        var host = response.address;
        var port = response.ports!.First().port;

        AddAllocationEntry($"{host}:{port}");
        return ($"http://{host}:{port}", responseJson);
    }

    /// <summary>
    /// Send Allocation request to Agones through Kubernetes Custom Resource Definition.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationCrdAsync(string @namespace, string fleetName)
    {
        var body = GameServerAllocationRequest.CreateRequest("allocation", fleetName);

        // ref: https://agones.dev/site/docs/guides/access-api/
        var json = JsonSerializer.Serialize(body);
        logger.LogDebug(json);

        var content = new StringContent(json);
        var response = await kubernetesClient.SendKubernetesApiAsync<GameServerAllocationResponse>($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", HttpMethod.Post, content);

        if (response == null) throw new ArgumentNullException(nameof(response));

        // debug output response JSON
        var responseJson = JsonSerializer.Serialize(response, jsonSerializerOptions);
        logger.LogDebug(responseJson);

        var host = response.status!.address;
        var port = response.status!.ports!.First().port;

        AddAllocationEntry($"{host}:{port}");
        return ($"http://{host}:{port}", responseJson);
    }

    /// <summary>
    /// Send Allocation request to Agones through Backend.
    /// </summary>
    /// <returns></returns>
    public async Task<(string endpoint, string response)> SendAllocationBackendAsync(string backendserverAddress)
    {
        var response = await agonesRpcClient.AllocateCrdAsync(backendserverAddress);

        var responseJson = JsonSerializer.Serialize(response, jsonSerializerOptions);
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
