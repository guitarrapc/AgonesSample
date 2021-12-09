using System.Net.Http.Headers;
using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Data;

public class AgonesAllocationService
{
    private readonly ILogger<AgonesAllocationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAgonesAllocationDatabase _database;
    private readonly string _endpoint;
    private readonly string _accessToken;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    public AgonesAllocationService(IAgonesAllocationDatabase database, IHttpClientFactory httpClientFactory, ILogger<AgonesAllocationService> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _database = database;
        _endpoint = KubernetesServiceProvider.Current.KubernetesServiceEndPoint;
        _accessToken = KubernetesServiceProvider.Current.AccessToken;
    }

    /// <summary>
    /// Send Get request to Kubernetes /api endpoint.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetKubernetesApiAsync() => SendKubernetesApiAsync("/api", HttpMethod.Get);

    /// <summary>
    /// Send Allocation request to Agones.
    /// </summary>
    /// <returns></returns>
    public async Task<string> SendAllocationAsync(string @namespace, string fleetName)
    {
        var body = GameServerAllocationRequest.CreateRequest("allocation", fleetName);
        var json = JsonSerializer.Serialize(body);
        // Console.WriteLine(json);

        var content = new StringContent(json);
        var response = await SendKubernetesApiAsync<GameServerAllocationResponse>($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", HttpMethod.Post, content);

        var host = response.status!.address;
        var port = response.status!.ports.First().port;
        AddAllocationEntry(new AgonesAllocation
        {
            Host = host,
            Port = port,
        });
        return $"http://{host}:{port}";
    }


    /// <summary>
    /// Send Request to Kubernetes API and get T result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private async Task<T> SendKubernetesApiAsync<T>(string path, HttpMethod method, HttpContent? content = null)
    {
        var httpClient = _httpClientFactory.CreateClient("kubernetes-api");
        var request = new HttpRequestMessage(method, $"{_endpoint}{path}");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_accessToken}");
        if (content is not null)
        {
            // agones can not accept content-type with media-type. 
            // re-apply request content-type to remove `media-type: utf8` from contet-type.
            // see: https://github.com/googleforgames/agones/blob/0e244fddf938e88dc5156ac2c7339adbb230daee/vendor/k8s.io/apimachinery/pkg/runtime/codec.go#L218-L220
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
        }
        var result = await httpClient.SendAsync(request);
        result.EnsureSuccessStatusCode();
        var json = await result.Content.ReadAsStringAsync();
        // Console.WriteLine(json);

        var bytes = await result.Content.ReadAsByteArrayAsync();
        var obj = JsonSerializer.Deserialize<T>(bytes, _serializerOptions);
        return obj!;
    }

    /// <summary>
    /// Send Request to Kubernetes API and get string result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    private async Task<string> SendKubernetesApiAsync(string path, HttpMethod method, HttpContent? content = null)
    {
        var httpClient = _httpClientFactory.CreateClient("kubernetes-api");
        var request = new HttpRequestMessage(method, $"{_endpoint}{path}");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_accessToken}");
        if (content is not null)
        {
            // agones can not accept content-type with media-type. 
            // re-apply request content-type to remove `media-type: utf8` from contet-type.
            // see: https://github.com/googleforgames/agones/blob/0e244fddf938e88dc5156ac2c7339adbb230daee/vendor/k8s.io/apimachinery/pkg/runtime/codec.go#L218-L220
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
        }
        var result = await httpClient.SendAsync(request);
        result.EnsureSuccessStatusCode();
        return await result.Content.ReadAsStringAsync();
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
