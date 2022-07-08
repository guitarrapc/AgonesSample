using System.Net.Http.Headers;
using System.Text.Json;
using SimpleShared;

namespace SimpleFrontEnd.Models;

public class KubernetesApiClient
{
    private readonly ILogger<KubernetesApiClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _endpoint;
    private readonly string _accessToken;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    public KubernetesApiClient(IHttpClientFactory httpClientFactory, ILogger<KubernetesApiClient> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _endpoint = KubernetesServiceProvider.Current.KubernetesServiceEndPoint;
        _accessToken = KubernetesServiceProvider.Current.AccessToken;
    }

    /// <summary>
    /// Send Request to Kubernetes API and get T result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async Task<ResponseT> SendKubernetesApiAsync<ResponseT>(string path, HttpMethod method, HttpContent? content = null)
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
        var obj = JsonSerializer.Deserialize<ResponseT>(bytes, _serializerOptions);
        return obj!;
    }

    /// <summary>
    /// Send Request to Kubernetes API and get string result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async Task<string> SendKubernetesApiAsync(string path, HttpMethod method, HttpContent? content = null)
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

}
