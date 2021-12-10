using Agones;

namespace SimpleGrpc.AgonesAspNetCore;

public class AgonesHostedService : IHostedService
{
    private readonly AgonesHealthKeeper _healthKeeper;
    private Task? _taskLoop;

    public AgonesHostedService(AgonesHealthKeeper healthKeeper)
    {
        _healthKeeper = healthKeeper;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // keep running health check loop.
        _taskLoop = _healthKeeper.ExecuteAsync();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_taskLoop is not null)
            await _taskLoop.ConfigureAwait(false);

        _healthKeeper.Dispose();
    }
}
