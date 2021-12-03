using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Data;

public class AgonesAllocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AgonesAllocationSet _allocationSet;
    private readonly string _endpoint;
    private readonly string _accessToken;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public AgonesAllocationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _allocationSet = new AgonesAllocationSet();
        _endpoint = KubernetesServiceProvider.Current.KubernetesServiceEndPoint;
        _accessToken = KubernetesServiceProvider.Current.AccessToken;
    }

    /// <summary>
    /// Kubernetes の /api エンドポイントに Get します。
    /// </summary>
    /// <returns></returns>
    public Task<string> GetKubernetesApiAsync() => SendKubernetesApiAsync("/api", HttpMethod.Get);

    /// <summary>
    /// Kubernetes API を実行します。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private async Task<T?> SendKubernetesApiAsync<T>(string path, HttpMethod method)
    {
        var httpClient = _httpClientFactory.CreateClient("kubernetes-api");
        var request = new HttpRequestMessage(method, $"{_endpoint}{path}");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_accessToken}");
        var result = await httpClient.SendAsync(request);
        result.EnsureSuccessStatusCode();
        var bytes = await result.Content.ReadAsByteArrayAsync();
        var obj = JsonSerializer.Deserialize<T>(bytes, _serializerOptions);
        return obj;
    }

    /// <summary>
    /// Kubernetes API を実行します。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private async Task<string> SendKubernetesApiAsync(string path, HttpMethod method)
    {
        var httpClient = _httpClientFactory.CreateClient("kubernetes-api");
        var request = new HttpRequestMessage(method, $"{_endpoint}{path}");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_accessToken}");
        var result = await httpClient.SendAsync(request);
        result.EnsureSuccessStatusCode();
        var bytes = await result.Content.ReadAsByteArrayAsync();
        return JsonSerializer.Serialize(bytes, _serializerOptions);
    }

    #region AllocationEntry

    public void AddAllocationEntry(string item) => _allocationSet.Add(item);
    public void AddAllocationEntry(AgonesAllocation item) => _allocationSet.Add($"{item.Host}:{item.Port}");
    public void ClearAllocationEntries() => _allocationSet.Clear();

    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string[] GetAllAllocationEntries()
    {
        if (_allocationSet.Count == 0)
            return Array.Empty<string>();

        return _allocationSet.Items.Select(x => $"http://{x}").ToArray();
    }
    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string? GetAllocationEntryRandomOrDefault()
    {
        if (_allocationSet.Count == 0)
            return null;

        var skip = Random.Shared.Next(0, _allocationSet.Count);
        var item = _allocationSet.Items.Skip(skip).First();
        return $"http://{item}";
    }
    #endregion

}
