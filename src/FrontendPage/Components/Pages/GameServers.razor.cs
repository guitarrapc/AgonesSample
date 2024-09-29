using FrontendPage.Clients;
using FrontendPage.Data;
using FrontendPage.Services;
using Microsoft.AspNetCore.Components;

namespace FrontendPage.Components.Pages;

public partial class GameServers : ComponentBase
{
    [Inject]
    public AgonesMagicOnionClient AgonesServerService { get; set; } = default!;
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;
    [Inject]
    public AgonesGameServerService AgonesGameServerService { get; set; } = default!;

    public string Message { get; set; } = "";
    public string Detail { get; set; } = "";
    public GameServerViewModel[] List { get; set; } = Array.Empty<GameServerViewModel>();

    private async Task GetGameServersAsync()
    {
        try
        {
            var res = await AgonesGameServerService.GetGameServersAsync("default");
            var gameservers = res.response;
            var json = JsonHelper.PrettyPrint(res.json);

            Message = $"{gameservers?.items?.Length ?? 0} GameServers found.";
            Detail = json;
            List = gameservers?.items?.Length is not null
                ? gameservers.items.Select(x => new GameServerViewModel
                {
                    Address = x.status?.address,
                    Port = x.status?.ports?[0]?.port ?? 0,
                    // 2021-12-09T06:35:40Z
                    Age = x.metadata is not null ? ConvertToAge(DateTime.UtcNow - x.metadata.creationTimestamp) : ConvertToAge(TimeSpan.Zero),
                    Name = x.metadata?.name,
                    Namespace = x.metadata?.@namespace,
                    Node = x.status?.nodeName,
                    Status = x.status?.state,
                }).ToArray()
                : [];
        }
        catch (Exception ex)
        {
            Message = $"Could not retrieve GameServerList.";
            Detail = $"{ex.Message} {ex.StackTrace}";
            List = [];
        }

        StateHasChanged();
    }

    private async Task ShutdownAsync(string? input)
    {
        if (input is null) return;

        var endpoint = input.Trim();
        var result = await AgonesServerService.ShutdownAsync("http://" + endpoint);
        Message = result.Message;
        Detail = result.Detail;

        if (!result.IsSuccess)
        {
            StateHasChanged();
            return;
        }

        // Remove Allocation Entry
        AgonesAllocationService.RemoveAllocationEntry(endpoint);

        // refresh page
        await GetGameServersAsync();
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
    public string? Namespace { get; init; }
    public string? Name { get; init; }
    public string? Status { get; init; }
    public string? Endpoint => Address + ":" + Port;
    public string? Address { get; init; }
    public int Port { get; init; }
    public string? Node { get; init; }
    // 30s, 15m, 3h30m, 5d
    public string? Age { get; init; }
}
