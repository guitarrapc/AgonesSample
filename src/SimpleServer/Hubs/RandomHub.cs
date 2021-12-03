using MagicOnion.Server.Hubs;
using SimpleShared;

namespace SimpleServer.Hubs;

public class RandomHub : StreamingHubBase<IRandomHub, IRandomHubReciever>, IRandomHub
{
    private readonly string _id = Guid.NewGuid().ToString();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ILogger<RandomHub> _logger;

    private IGroup? _group;
    private Task? _taskLoop;

    public RandomHub(ILogger<RandomHub> logger)
    {
        _logger = logger;
    }

    public async Task<StartResult> StartAsync()
    {
        _logger.LogInformation($"StartAsync: {_id}; {Context.ContextId}");
        _group = await Group.AddAsync(_id);
        _taskLoop = Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(1 * 1000);
                Broadcast(_group).OnMessageRecieved($"{Environment.MachineName}|{_id}|{DateTime.Now}");
            }
        });

        return new StartResult(Environment.MachineName, _id);
    }

    protected override async ValueTask OnDisconnected()
    {
        _cts.Cancel();
        if (_taskLoop is not null)
        {
            await _taskLoop;
        }
    }
}
