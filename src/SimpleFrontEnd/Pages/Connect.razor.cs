using System.Collections.Immutable;
using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Data;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class Connect : ComponentBase, IRandomHubReciever, IDisposable
{
    [Inject]
    public AgonesAllocationSet AgonesAllocationList { get; set; } = default!;

    private ImmutableList<string> _log = ImmutableList<string>.Empty;
    private GrpcChannel? _channel;
    private IRandomHub? _hubClient;
    private bool _connected;
    private string? _host;
    private string? _id;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var address = AgonesAllocationList.GetAddressRandomOrDefault();
        if (address is null)
            return;

        _channel = GrpcChannel.ForAddress(address);
        _hubClient = await StreamingHubClient.ConnectAsync<IRandomHub, IRandomHubReciever>(_channel, this);
        var result = await _hubClient.StartAsync();
        _host = result.Host;
        _id = result.Id;

        _connected = true;

    }

    public void Dispose()
    {
        if (_hubClient is not null)
            _hubClient.DisposeAsync().Wait();

        _channel?.Dispose();
    }

    public void OnMessageRecieved(string message)
    {
        _log = _log.Insert(0, message);
        InvokeAsync(() => StateHasChanged());
    }
}
