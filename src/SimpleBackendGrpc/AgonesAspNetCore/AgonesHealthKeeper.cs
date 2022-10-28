using Agones;
using Microsoft.Extensions.Options;

namespace AgonesAspNetCore;

public class AgonesHealthKeeper : IAsyncDisposable
{
    private readonly IAgonesSDK _agonesSdk;
    private readonly IOptions<AgonesOptions> _option;
    private readonly AgonesCondition _condition;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger<AgonesHealthKeeper> _logger;

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _linkedCts;
    private Task? _taskLoop = null;

    public AgonesHealthKeeper(IAgonesSDK agonesSdk, IOptions<AgonesOptions> options, AgonesCondition condition, ILogger<AgonesHealthKeeper> logger)
    {
        _agonesSdk = agonesSdk;
        _option = options;
        _condition = condition;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        // initial delay before begin health check
        await Task.Delay(_option.Value.HealthcheckDelay).ConfigureAwait(false);

        _logger.LogInformation("Initializing AgonesSdk.");
        await InitializeAsync().ConfigureAwait(false);

        _logger.LogInformation("Begin health check keeper.");
        await KeepAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        _logger.LogInformation("Connect to AgonesSdk.");

        // Let's connect to Agones Sdk.
        var connected = await _agonesSdk.ConnectAsync().ConfigureAwait(false);
        _condition.Connected(connected);
        if (!connected)
        {
            _logger.LogInformation("AgonesSdk connection failed.");
            throw new OperationCanceledException(nameof(AgonesHostedService));
        }

        // Set Status to ready.
        _logger.LogInformation("AgonesSdk connection success change state to Ready.");
        await _agonesSdk.ReadyAsync().ConfigureAwait(false);
        _condition.UpdateState(AgonesState.Ready);
    }

    private Task KeepAsync()
    {
        _logger.LogInformation("Health checking with AgonesSdk.");

        // check both AgonesSdk's cancellation token and Keeper's cancellation token.
        _timer = new PeriodicTimer(_option.Value.HealthcheckInterval);
        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, _option.Value.SdkCancellationTokenSource.Token);
        _taskLoop = Task.Run(async () =>
        {
            // loop to keep health check
            while (!_linkedCts.IsCancellationRequested)
            {
                try
                {
                    var status = await _agonesSdk.HealthAsync().ConfigureAwait(false);
                    _condition.Healthy(status.StatusCode);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }

                await _timer.WaitForNextTickAsync().ConfigureAwait(false);
            }
        });

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();

        if (_option.Value.SdkCancellationTokenSource is not null)
            _option.Value.SdkCancellationTokenSource.Cancel();

        if (_taskLoop is not null)
            await _taskLoop.ConfigureAwait(false);

        _linkedCts?.Dispose();
        _timer?.Dispose();
    }
}
