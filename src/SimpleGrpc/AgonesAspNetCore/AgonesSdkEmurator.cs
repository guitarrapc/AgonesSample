using Agones;
using Agones.Dev.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace AgonesAspNetCore;

/// <summary>
/// AgonesSDK Emurator for non-Kubernetes Environment.
/// </summary>
public class AgonesEmuratedSdk : IAgonesSDK
{
    private readonly ILogger? _logger;
    private AgonesState _status;
    private bool _connected;
    private bool _healthChecked;
    private readonly GameServer _gameServer;

    private bool IsShutdowned => _status == AgonesState.Shutdown;

    public AgonesEmuratedSdk(IOptions<AgonesOptions>? options = null, ILogger? logger = null)
    {
        _logger = logger;
        _status = AgonesState.Scheduled;
        _connected = false;
        _healthChecked = false;
        _gameServer = new GameServer()
        {
            ObjectMeta = new GameServer.Types.ObjectMeta
            {
                Name = "DummyGameServer",
                Namespace = "default",
            },
            Spec = new GameServer.Types.Spec
            {
                Health = new GameServer.Types.Spec.Types.Health
                {
                    InitialDelaySeconds = 5,
                    PeriodSeconds = 5,
                    FailureThreshold = 3,
                },
            },
            Status = new GameServer.Types.Status
            {
                Address = AgonesHelper.GetLocalIPv4(),
                State = AgonesState.Scheduled.ToString(),
            },
        };
        _gameServer.Status.Ports.Add(new GameServer.Types.Status.Types.Port
        {
            Name = "default",
            Port_ = options?.Value.EmulatedSdkPort ?? 0,
        });
    }

    public Task<Status> AllocateAsync()
    {
        _logger?.LogDebug($"{nameof(AllocateAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Allocated;
        return Task.FromResult(Status.DefaultSuccess);
    }

    // https://agones.dev/site/docs/guides/client-sdks/#alphaplayerconnectplayerid
    public IAgonesAlphaSDK Alpha()
    {
        _logger?.LogDebug($"{nameof(Alpha)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        throw new NotImplementedException();
    }

    public Task<bool> ConnectAsync()
    {
        _logger?.LogDebug($"{nameof(ConnectAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");

        _connected = true;
        _status = AgonesState.Scheduled;
        return Task.FromResult(true);
    }

    public void Dispose()
    {
    }

    public Task<GameServer> GetGameServerAsync()
    {
        _logger?.LogDebug($"{nameof(GetGameServerAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        return Task.FromResult(_gameServer);
    }

    public Task<Status> HealthAsync()
    {
        _logger?.LogDebug($"{nameof(HealthAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        _healthChecked = true;
        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> ReadyAsync()
    {
        _logger?.LogDebug($"{nameof(ReadyAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Ready;
        _gameServer.Status.State = AgonesState.Ready.ToString();
        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> ReserveAsync(long seconds)
    {
        _logger?.LogDebug($"{nameof(ReserveAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Reserved;
        _gameServer.Status.State = AgonesState.Reserved.ToString();
        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> SetAnnotationAsync(string key, string value)
    {
        _logger?.LogDebug($"{nameof(SetAnnotationAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        if (!_gameServer.ObjectMeta.Annotations.TryAdd(key, value))
        {
            _gameServer.ObjectMeta.Annotations[key] = value;
        }

        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> SetLabelAsync(string key, string value)
    {
        _logger?.LogDebug($"{nameof(SetLabelAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        if (!_gameServer.ObjectMeta.Labels.TryAdd(key, value))
        {
            _gameServer.ObjectMeta.Labels[key] = value;
        }

        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> ShutDownAsync()
    {
        // You can recover via calling ConnectAsync.
        _logger?.LogDebug($"{nameof(ShutDownAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Shutdown;
        return Task.FromResult(Status.DefaultSuccess);
    }

    public void WatchGameServer(Action<GameServer> callback)
    {
        _logger?.LogDebug($"{nameof(WatchGameServer)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        callback(_gameServer);
    }
}
