using Agones;
using Agones.Dev.Sdk;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace AgonesAspNetCore;

/// <summary>
/// Presudo AgonesSDK for non-Kubernetes Environment.
/// </summary>
public class AgonesSDKPresudo : IAgonesSDK
{
    private readonly IOptions<AgonesOptions> _options;
    private readonly ILogger? _logger;
    private AgonesState _status;
    private bool _connected;
    private bool _healthChecked;
    private readonly GameServer _gameServer;

    private bool IsShutdowned => _status == AgonesState.Shutdown;

    public AgonesSDKPresudo(IOptions<AgonesOptions> options, ILogger? logger = null)
    {
        _options = options;
        _logger = logger;
        _status = AgonesState.Scheduled;
        _connected = false;
        _healthChecked = false;

        var fleetName = options.Value.EmulateSdkFleetName.ToLower();
        var (gssName, gsName) = GetGameServerNames(fleetName);

        _gameServer = new GameServer()
        {
            ObjectMeta = new GameServer.Types.ObjectMeta
            {
                Name = gsName,
                Namespace = options.Value.EmulateSdkNameSpace,
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
        _gameServer.ObjectMeta.Labels.TryAdd("agones.dev/fleet", fleetName);
        _gameServer.ObjectMeta.Labels.TryAdd("agones.dev/gameserverset", gssName);
        _gameServer.Status.Ports.Add(new GameServer.Types.Status.Types.Port
        {
            Name = "default",
            Port_ = options.Value.EmulateSdkPort,
        });
    }

    public Task<Status> AllocateAsync()
    {
        _logger?.LogDebug($"{nameof(AllocateAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Allocated;
        _gameServer.Status.State = AgonesState.Allocated.ToString();
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
        _gameServer.Status.State = AgonesState.Scheduled.ToString();
        return Task.FromResult(true);
    }

    public void Dispose()
    {
    }

    public Task<GameServer> GetGameServerAsync()
    {
        _logger?.LogDebug($"{nameof(GetGameServerAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

        return Task.FromResult(_gameServer);
    }

    public Task<Status> HealthAsync()
    {
        _logger?.LogDebug($"{nameof(HealthAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
        {
            _gameServer.Status.State = AgonesState.Unhealthy.ToString();
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);
        }

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
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Reserved;
        _gameServer.Status.State = AgonesState.Reserved.ToString();
        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> SetAnnotationAsync(string key, string value)
    {
        _logger?.LogDebug($"{nameof(SetAnnotationAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

        if (!_gameServer.ObjectMeta.Annotations.TryAdd(key, value))
        {
            _gameServer.ObjectMeta.Annotations[key] = value;
        }

        return Task.FromResult(Status.DefaultSuccess);
    }

    public Task<Status> SetLabelAsync(string key, string value)
    {
        _logger?.LogDebug($"{nameof(SetLabelAsync)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

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
        if (IsShutdowned)
            throw new HttpRequestException($"AgonesSDK already shutdowned.", null, System.Net.HttpStatusCode.BadRequest);
        if (!_connected)
            throw new HttpRequestException($"Please run {nameof(ConnectAsync)} before call method.", null, System.Net.HttpStatusCode.BadRequest);

        _status = AgonesState.Shutdown;
        _gameServer.Status.State = AgonesState.Shutdown.ToString();
        if (!_options?.Value.EmulateSdkNoShutdown ?? false)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(3 * 1000);
                Environment.Exit(99); // Let's indicate custom error code for docker restart.
            });
        }
        return Task.FromResult(Status.DefaultSuccess);
    }

    public void WatchGameServer(Action<GameServer> callback)
    {
        _logger?.LogDebug($"{nameof(WatchGameServer)} called. status {_status}; connected {_connected}; healthChecked {_healthChecked};");
        callback(_gameServer);
    }

    /// <summary>
    /// Get GameServer Name and GameServerSet Name.
    /// Fleet -> GameServerSet -> GameServer
    /// </summary>
    /// <param name="fleetName"></param>
    /// <returns></returns>
    private (string GameServerSetName, string GameServerName) GetGameServerNames(string fleetName)
    {
        // generate random string for specific length.
        string HashString(string text, int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(bytes);

            var outputHash = new char[length];
            for (var i = 0; i < outputHash.Length; i++)
            {
                outputHash[i] = chars[hash[i] % chars.Length];
            }

            return new string(outputHash);
        }

        // always generate random when called
        var num = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 1000000);

        var gssName = fleetName + "-" + HashString(fleetName, 5);
        var gsName = gssName + "-" + HashString(num.ToString(), 5);
        return (gssName, gsName);
    }
}
