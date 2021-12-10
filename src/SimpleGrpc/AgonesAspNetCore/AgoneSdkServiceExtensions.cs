using Agones;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SimpleGrpc.AgonesAspNetCore;

public static class AgoneSdkServiceExtensions
{
    public static IAgonesSdkBuilder AddAgonesSdk(this IServiceCollection services, Action<AgonesSDK, AgonesOption>? configure = null)
    {
        return services.AddAgonesSdkCore(configure);
    }

    private static IAgonesSdkBuilder AddAgonesSdkCore(this IServiceCollection services, Action<AgonesSDK, AgonesOption>? configure)
    {
        // Configure
        var sdk = new AgonesSDK();
        var option = new AgonesOption();
        if (configure != null)
        {
            configure(sdk, option);
        }

        // Add DI
        services.TryAddSingleton(sdk);
        services.TryAddSingleton(option);
        services.TryAddSingleton<AgonesCondition>();
        services.TryAddSingleton<AgonesHealthKeeper>();

        // Add HostedService
        services.AddHostedService<AgonesHostedService>();

        return new AgonesSdkBuilder(services);
    }
}
