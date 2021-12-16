using Agones;

namespace SimpleGrpc.AgonesAspNetCore;

public class AgonesHostedService : IHostedService
{
    private readonly AgonesHealthKeeper _healthKeeper;
    private Task? _task;

    public AgonesHostedService(AgonesHealthKeeper healthKeeper)
    {
        _healthKeeper = healthKeeper;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // keep running health check loop.
        _task = _healthKeeper.ExecuteAsync();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _healthKeeper.DisposeAsync();
        if (_task is not null)
            await _task.ConfigureAwait(false);
    }
}
