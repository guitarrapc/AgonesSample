using Agones;
using AgonesAspNetCore;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration.Memory;
using Shared;

namespace BackendServerGrpc.AgonesAspNetCore;

public class AgonesStartupService(IAgonesSDK sdk, IHostApplicationLifetime applicationLifetime, IServer server, IConfiguration configuration, ILogger<AgonesStartupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Other servers in same process can access to IAgonesSDK through Accessor.
        AgonesSDKAccessor.SetAgonesSDK(sdk);

        if (!KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            applicationLifetime.ApplicationStarted.Register(RewriteEmulationPort);
        }
    }

    private void RewriteEmulationPort()
    {
        if (configuration is IConfigurationRoot configurationRoot)
        {
            // Re-write the emulation port number to use one of the server listening on.
            var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses ?? Array.Empty<string>();
            if (addresses.Any())
            {
                var port = addresses.First().Split(":")[^1];
                var memoryConfigProvider = configurationRoot.Providers
                    .OfType<MemoryConfigurationProvider>()
                    .FirstOrDefault();
                if (memoryConfigProvider is not null)
                {
                    var key = $"{nameof(AgonesOptions)}:{nameof(AgonesOptions.EmulateSdkPort)}";
                    logger.LogInformation($"{key} is re-written to {port}.");
                    memoryConfigProvider.Set(key, port);
                    configurationRoot.Reload();
                }
            }
        }
    }
}
