using System.Net.Http.Headers;
using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class AgonesAllocatorApiClient
{
    private readonly ILogger<AgonesAllocatorApiClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false,
    };

    public AgonesAllocatorApiClient(IHttpClientFactory httpClientFactory, ILogger<AgonesAllocatorApiClient> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Send Request to Kubernetes API and get T result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async Task<AgonesAllocationApiResponse> SendAllocationApiAsync(string endpoint, AgonesAllocationApiRequest body)
    {
        var requestJson = JsonSerializer.Serialize(body, _serializerOptions);
        _logger.LogDebug(requestJson);
        var content = new StringContent(requestJson);

        var httpClient = _httpClientFactory.CreateClient("agonesallocator-api");
        var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/gameserverallocation");
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        // agones can not accept content-type with media-type. 
        // re-apply request content-type to remove `media-type: utf8` from contet-type.
        // see: https://github.com/googleforgames/agones/blob/0e244fddf938e88dc5156ac2c7339adbb230daee/vendor/k8s.io/apimachinery/pkg/runtime/codec.go#L218-L220
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        var res = await httpClient.SendAsync(request);
        //result.EnsureSuccessStatusCode();


        var bytes = await res.Content.ReadAsByteArrayAsync();
        var response = JsonSerializer.Deserialize<AgonesAllocationApiResponse>(bytes, _serializerOptions);

        if (response == null || !string.IsNullOrEmpty(response.error))
        {
            var json = await res.Content.ReadAsStringAsync();
            _logger.LogDebug(json);
            throw new HttpRequestException($"Status Code indicated failure {res.StatusCode} {(int)res.StatusCode}. {json}", null, res.StatusCode);
        }
        return response;
    }
}
