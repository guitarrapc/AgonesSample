using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Models;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class CallService : ComponentBase
{
    [Inject]
    public AgonesAllocationService AgonesAllocationService { get; set; } = default!;

    private string? _address;
    public string Result { get; set; } = "";
    public string Input { get; set; } = "";

    private async Task CallServiceAsync()
    {
        var address = AgonesAllocationService.GetAllocationEntryRandomOrDefault();
        if (address is null)
            return;

        _address = address;
        using var channel = GrpcChannel.ForAddress(address);
        var echoService = MagicOnionClient.Create<IEchoService>(channel);
        Result = await echoService.EchoAsync(Input);
    }
}
