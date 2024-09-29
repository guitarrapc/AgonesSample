using Shared.AgonesApi;
using System.Text.Json;

namespace FrontendPage.Clients;

public class AgonesApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    /// <summary>
    /// Send Request to Kubernetes API and get T result.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<string> AllocateAsync(string endpoint, GameServerAllocationApiRequest body)
    {
        var httpClient = CreateClient();

        // Endpoint should be `http://agones-allocator.agones-system.svc.cluster.local:8443/gameserverallocation`
        // HACK:agones can not accept content-type with media-type. re-apply request content-type to remove `media-type: utf8` from contet-type.
        // see: https://github.com/googleforgames/agones/blob/0e244fddf938e88dc5156ac2c7339adbb230daee/vendor/k8s.io/apimachinery/pkg/runtime/codec.go#L218-L220
        var response = await httpClient.PostAsJsonAsync($"{endpoint}/gameserverallocation", body, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return json;
    }

    private HttpClient CreateClient() => httpClientFactory.CreateClient("agones-api");
}
