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
    private readonly AgonesCondition _condition;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IOptionsMonitor<AgonesOptions> _options;
    private readonly ILogger<AgonesSDKPresudo> _logger;
    private bool _disposed; // connection status
    private GameServer _gameServer;
    private readonly object _lock = new();

    public AgonesSDKPresudo(AgonesCondition condition, IHostApplicationLifetime lifetime, IOptionsMonitor<AgonesOptions> options, ILogger<AgonesSDKPresudo> logger)
    {
        _condition = condition;
        _lifetime = lifetime;
        _options = options;
        _logger = logger;
        _disposed = false;

        _gameServer = InitGameServer(options.CurrentValue);
        options.OnChange(x => _gameServer = InitGameServer(x));
    }

    private GameServer InitGameServer(AgonesOptions options)
    {
        var fleetName = options.EmulateSdkFleetName.ToLower();
        var (gssName, gsName) = GetGameServerNames(fleetName);

        var gs = new GameServer()
        {
            ObjectMeta = new GameServer.Types.ObjectMeta
            {
                Name = gsName,
                Namespace = options.EmulateSdkNameSpace,
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
        gs.ObjectMeta.Labels.TryAdd("agones.dev/fleet", fleetName);
        gs.ObjectMeta.Labels.TryAdd("agones.dev/gameserverset", gssName);
        gs.Status.Ports.Add(new GameServer.Types.Status.Types.Port
        {
            Name = "default",
            Port_ = options.EmulateSdkPort,
        });

        return gs;
    }

    public async Task<Status> AllocateAsync()
    {
        _logger.LogDebug($"{nameof(AllocateAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        _gameServer.Status.State = AgonesState.Allocated.ToString();
        _condition.SetAllocated();

        return Status.DefaultSuccess;
    }

    /// <summary>
    /// https://github.com/googleforgames/agones/blob/main/sdks/csharp/sdk/Alpha.cs
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IAgonesAlphaSDK Alpha()
    {
        _logger.LogDebug($"{nameof(Alpha)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        throw new NotImplementedException();
    }
    /// <summary>
    /// https://github.com/googleforgames/agones/blob/main/sdks/csharp/sdk/Beta.cs
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IAgonesBetaSDK Beta()
    {
        _logger.LogDebug($"{nameof(Beta)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        throw new NotImplementedException();
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AgonesSDKPresudo));
            _disposed = true;
        }
    }

    public async Task<GameServer> GetGameServerAsync()
    {
        _logger.LogDebug($"{nameof(GetGameServerAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        return _gameServer;
    }

    public async Task<Status> HealthAsync()
    {
        _logger.LogDebug($"{nameof(HealthAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        return Status.DefaultSuccess;
    }

    public async Task<Status> ReadyAsync()
    {
        _logger.LogDebug($"{nameof(ReadyAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        _gameServer.Status.State = AgonesState.Ready.ToString();
        _condition.SetReady();

        return Status.DefaultSuccess;
    }

    public async Task<Status> ReserveAsync(long seconds)
    {
        _logger.LogDebug($"{nameof(ReserveAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        _gameServer.Status.State = AgonesState.Reserved.ToString();
        _condition.SetReserved();

        return Status.DefaultSuccess;
    }

    public async Task<Status> SetAnnotationAsync(string key, string value)
    {
        _logger.LogDebug($"{nameof(SetAnnotationAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        _gameServer.ObjectMeta.Annotations[key] = value;

        return Status.DefaultSuccess;
    }

    public async Task<Status> SetLabelAsync(string key, string value)
    {
        _logger.LogDebug($"{nameof(SetLabelAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        _gameServer.ObjectMeta.Labels[key] = value;

        return Status.DefaultSuccess;
    }

    public async Task<Status> ShutDownAsync()
    {
        _logger.LogDebug($"{nameof(ShutDownAsync)} called. status {_gameServer.Status.State};");
        ValidateAvailable();

        lock (_lock)
        {
            if (_condition.State != AgonesState.Shutdown)
            {
                _gameServer.Status.State = AgonesState.Shutdown.ToString();
                _condition.SetShutdown();

                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));

                    // Let's indicate custom error code for container restart.
                    Environment.ExitCode = 99;

                    // Stop server application
                    _lifetime.StopApplication();
                });
            }
        }

        return Status.DefaultSuccess;
    }

    public void WatchGameServer(Action<GameServer> callback)
    {
        _logger.LogDebug($"{nameof(WatchGameServer)} called. status {_gameServer.Status.State};");
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

    private void ValidateAvailable()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AgonesSDKPresudo));
    }
}
