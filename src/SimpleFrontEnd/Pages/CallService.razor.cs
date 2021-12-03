using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.AspNetCore.Components;
using SimpleFrontEnd.Data;
using SimpleShared;

namespace SimpleFrontEnd.Pages;

public partial class CallService : ComponentBase
{
    [Inject]
    public AgonesAllocationSet AgonesAllocationList { get; set; } = default!;

    public string Result { get; set; } = "";
    public string Input { get; set; } = "";

    private async Task CallServiceAsync()
    {
        var address = AgonesAllocationList.GetAddressRandomOrDefault();
        if (address is null)
            return;

        using var channel = GrpcChannel.ForAddress(address);
        var echoService = MagicOnionClient.Create<IEchoService>(channel);
        Result = await echoService.EchoAsync(Input);
    }
}
