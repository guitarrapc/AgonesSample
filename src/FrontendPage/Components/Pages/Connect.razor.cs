using FrontendPage.Infrastructures;
using FrontendPage.Services;
using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components;
using Shared;
using System.Collections.Immutable;

namespace FrontendPage.Components.Pages;

public partial class Connect : ComponentBase, IRandomHubReciever, IAsyncDisposable
{
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    private ImmutableList<string> _log = ImmutableList<string>.Empty;
    private GrpcChannel? _channel;
    private IRandomHub? _hubClient;
    private bool _connected;
    private string? _address;
    private string? _host;
    private Guid? _id;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var address = AgonesAllocationService.GetAllocationEntryRandomOrDefault();
        if (address is null)
            return;

        _channel = GrpcChannelPool.Instance.CreateChannel(address);
        _hubClient = await StreamingHubClient.ConnectAsync<IRandomHub, IRandomHubReciever>(_channel, this);
        await _hubClient.JoinAsync(new JoinRequest
        {
            RoomName = "Agones",
            UserName = Environment.MachineName,
        });
        var result = await _hubClient.StartAsync();
        _address = address;
        _host = result.Host;
        _id = result.Id;

        _connected = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubClient is not null)
        {
            await _hubClient.DisposeAsync();
        }
    }


    public void OnJoin(string userName)
    {
        _log = _log.Insert(0, userName);
        InvokeAsync(() => StateHasChanged());
    }

    public void OnMessageRecieved(string message)
    {
        _log = _log.Insert(0, message);
        InvokeAsync(() => StateHasChanged());
    }
}
