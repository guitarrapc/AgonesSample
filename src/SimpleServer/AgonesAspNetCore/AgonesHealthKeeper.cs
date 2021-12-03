using Agones;
using SimpleShared;

namespace SimpleServer.AgonesAspNetCore;

public class AgonesHealthKeeper : IDisposable
{
    private readonly AgonesSDK _agonesSdk;
    private readonly AgonesOption _option;
    private readonly AgonesCondition _condition;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger<AgonesHealthKeeper> _logger;

    private Task? _taskLoop = null;

    public AgonesHealthKeeper(AgonesSDK agonesSdk, AgonesOption option, AgonesCondition condition, ILogger<AgonesHealthKeeper> logger)
    {
        _agonesSdk = agonesSdk;
        _option = option;
        _condition = condition;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        // initial delay before begin health check
        await Task.Delay(_option.HealthcheckDelay).ConfigureAwait(false);

        _logger.LogInformation("Initializing AgonesSdk.");
        await InitializeAsync().ConfigureAwait(false);

        _logger.LogInformation("Begin health check keeper.");
        await KeepAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        // Agones should run when running on Kubernetes. Otherwise ignore it.
        // todo: emulate agones sdk for local usage.
        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            _logger.LogInformation("Not running on Kubernetes, emulate AgonesSdk.");
            _condition.IsEmulating = true;
            _condition.IsConnected = true;
            _condition.State = AgonesState.Ready;
        }
        else
        {
            _logger.LogInformation("Running on Kubernetes, Connect to AgonesSdk.");

            // Let's connect to Agones Sdk.
            var connected = await _agonesSdk.ConnectAsync().ConfigureAwait(false);
            _condition.IsConnected = connected;
            if (!connected)
            {
                _logger.LogInformation("AgonesSdk connection failed.");
                throw new OperationCanceledException(nameof(AgonesHostedService));
            }
            _condition.State = AgonesState.Scheduled;

            // Set Status to ready.
            _logger.LogInformation("AgonesSdk connection success change state to Ready.");
            await _agonesSdk.ReadyAsync().ConfigureAwait(false);
            _condition.State = AgonesState.Ready;
        }
    }

    private Task KeepAsync()
    {
        if (_condition.IsEmulating)
        {
            _logger.LogInformation("You are running on emulation mode, stop health check loop.");
            _condition.HealthStatus = true;
            return Task.CompletedTask;
        }

        _taskLoop = Task.Run(async () =>
        {
            _logger.LogInformation("Health checking with Agones-sdk sidecar.");

            // loop to keep health check
            while (!_cts.IsCancellationRequested)
            {
                if (_condition.IsConnected)
                {
                    var status = await _agonesSdk.HealthAsync().ConfigureAwait(false);
                    _condition.HealthStatus = status.StatusCode == Grpc.Core.StatusCode.OK;
                }
                else
                {
                    _logger.LogInformation("Still not connected with AgonesSdk, wait for next loop.");
                    _condition.HealthStatus = false;
                }

                await Task.Delay(_option.HealthcheckInterval).ConfigureAwait(false);
            }
        });

        return Task.CompletedTask;
    }

    public async void Dispose()
    {
        _cts.Cancel();
        if (_taskLoop is not null)
            await _taskLoop.ConfigureAwait(false);
    }
}
