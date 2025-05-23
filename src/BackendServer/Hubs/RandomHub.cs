using Cysharp.Runtime.Multicast;
using MagicOnion.Server.Hubs;
using Shared;

namespace BackendServerGrpc.Hubs;

public class RandomHub : StreamingHubBase<IRandomHub, IRandomHubReciever>, IRandomHub
{
    private CancellationTokenSource? _cts;
    private readonly ILogger<RandomHub> _logger;
    private readonly IMulticastSyncGroup<Guid, IRandomHubReciever> _roomForAll;

    private IGroup<IRandomHubReciever>? _room;
    private string? _myName;
    private Task? _taskLoop;

    public RandomHub(IMulticastGroupProvider groupProvider, ILogger<RandomHub> logger)
    {
        _logger = logger;
        _roomForAll = groupProvider.GetOrAddSynchronousGroup<Guid, IRandomHubReciever>("All");
    }

    public async Task JoinAsync(JoinRequest request)
    {
        _logger.LogInformation($"{nameof(JoinAsync)}: {request.RoomName}; {ConnectionId}; {Context.ContextId}");
        _room = await Group.AddAsync(request.RoomName);
        _myName = request.UserName;
        _room.All.OnJoin(request.UserName);
    }

    public async Task<StartResult> StartAsync()
    {
        _logger.LogInformation($"{nameof(StartAsync)}: {ConnectionId}; {Context.ContextId}");

        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        _taskLoop = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(1 * 1000);
                _room?.All.OnMessageRecieved($"{Environment.MachineName}|{ConnectionId}|{DateTime.Now}");
            }
        }, ct);

        return new StartResult
        {
            Host = Environment.MachineName,
            Id = ConnectionId,
        };
    }

    protected override async ValueTask OnDisconnected()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        if (_taskLoop is not null)
        {
            await _taskLoop;
        }
        _roomForAll.Remove(ConnectionId);
    }
}
