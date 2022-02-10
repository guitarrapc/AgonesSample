using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Models;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class GameServers : ComponentBase
{
    [Inject]
    public AgonesGameServerService AgonesGameServerService { get; set; } = default!;
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = default!;

    public string? Result { get; set; }
    public GameServerViewModel[] List { get; set; } = Array.Empty<GameServerViewModel>();

    private async Task GetGameServersAsync()
    {
        try
        {
            if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
            {
                var gameservers = await AgonesGameServerService.GetGameServersAgonesAsync();
                Result = "Frontend is not running Kubernetse. You cannot execute allocate.";
                List = gameservers?.items is not null
                    ? gameservers!.items.Select(x => new GameServerViewModel
                    {
                        Address = x.status!.address,
                        Port = x.status.ports![0].port,
                    // 2021-12-09T06:35:40Z
                    Age = ConvertToAge(DateTime.UtcNow - x.metadata!.creationTimestamp),
                        Name = x.metadata.name,
                        Namespace = x.metadata.@namespace,
                        Node = x.status.nodeName,
                        Status = x.status.state,
                    }).ToArray()
                    : Array.Empty<GameServerViewModel>();
            }
            else
            {
                var gameservers = await AgonesGameServerService.GetGameServersAsync("default");
                Result = $"{gameservers?.items?.Length ?? 0} GameServers found.";
                List = gameservers?.items is not null
                    ? gameservers!.items.Select(x => new GameServerViewModel
                    {
                        Address = x.status!.address,
                        Port = x.status.ports![0].port,
                        // 2021-12-09T06:35:40Z
                        Age = ConvertToAge(DateTime.UtcNow - x.metadata!.creationTimestamp),
                        Name = x.metadata.name,
                        Namespace = x.metadata.@namespace,
                        Node = x.status.nodeName,
                        Status = x.status.state,
                    }).ToArray()
                    : Array.Empty<GameServerViewModel>();
            }
        }
        catch (Exception ex)
        {
            Result = $"Could not retrieve GameServerList. {ex.Message}";
            List = Array.Empty<GameServerViewModel>();
        }

        StateHasChanged();
    }

    private string ConvertToAge(TimeSpan timeSpan)
    {
        if (timeSpan < TimeSpan.FromMinutes(1))
        {
            return $"{(int)timeSpan.TotalSeconds}s";
        }
        else if (timeSpan < TimeSpan.FromHours(5))
        {
            return $"{(int)timeSpan.TotalMinutes}m";
        }
        else if (timeSpan < TimeSpan.FromDays(1))
        {
            return $"{(int)timeSpan.Hours}h {(int)timeSpan.Minutes}m";
        }
        else
        {
            return $"{(int)timeSpan.TotalDays}d";
        }
    }
}

public class GameServerViewModel
{
    public string? Namespace { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Address { get; set; }
    public int Port { get; set; }
    public string? Node { get; set; }
    // 30s, 15m, 3h30m, 5d
    public string? Age { get; set; }
}
