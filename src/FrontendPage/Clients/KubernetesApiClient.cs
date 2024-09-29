using Shared.AgonesCrd;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrontendPage.Clients;

public class KubernetesApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    /// <summary>
    /// Allocate GameServer
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task<string> AllocateAsync(string @namespace, GameServerAllocationCrdRequest body)
    {
        var response = await PostAsync($"/apis/allocation.agones.dev/v1/namespaces/{@namespace}/gameserverallocations", body);
        return response;
    }

    /// <summary>
    /// Get GameServers
    /// </summary>
    /// <param name="namespace"></param>
    /// <returns></returns>
    public async Task<string> GetGameServersAsync(string @namespace)
    {
        return await GetAsync($"/apis/agones.dev/v1/namespaces/{@namespace}/gameservers");
    }

    /// <summary>
    /// Get Kubernetes API Root
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetKubernetesApiRoot()
    {
        return await GetAsync("/api");
    }

    /// <summary>
    /// Post Request to Kubernetes API
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="path"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<string> PostAsync<TRequest>(string path, TRequest request)
    {
        var httpClient = CreateClient();
        var response = await httpClient.PostAsJsonAsync(path, request, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }

    /// <summary>
    /// Get Request to Kubneretes API
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private async Task<string> GetAsync(string path)
    {
        var httpClient = CreateClient();
        var json = await httpClient.GetStringAsync(path);
        return json;
    }

    private HttpClient CreateClient() => httpClientFactory.CreateClient("kubernetes-api");
}
