using Agones;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleShared;

namespace AgonesAspNetCore;

public static class AgoneSdkServiceExtensions
{
    public static IAgonesSdkBuilder AddAgonesSdk(this IServiceCollection services, Action<AgonesOptions>? configureOptions = null)
    {
        return services.AddAgonesSdkCore(configureOptions);
    }

    private static IAgonesSdkBuilder AddAgonesSdkCore(this IServiceCollection services, Action<AgonesOptions>? configureOptions)
    {
        var configName = Options.DefaultName;

        // Add DI
        services.TryAddSingleton<IAgonesSDK>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("AgonesSdk");
            var options = sp.GetRequiredService<IOptions<AgonesOptions>>();

            IAgonesSDK sdk = KubernetesServiceProvider.Current.IsRunningOnKubernetes
                ? new AgonesSDK(cancellationTokenSource: options.Value.SdkCancellationTokenSource, logger: logger)
                : new AgonesSDKPresudo(options, logger);

            // Other servers in same process can access to IAgonesSDK through Accessor.
            AgonesSDKAccessor.SetAgonesSDK(sdk);
            return sdk;
        });
        services.TryAddSingleton<AgonesCondition>();
        services.TryAddSingleton<AgonesHealthKeeper>();

        services.AddOptions<AgonesOptions>(configName)
            .Configure<IConfiguration>((o, configuration) =>
            {
                configuration.GetSection(string.IsNullOrWhiteSpace(configName) ? "AgonesSdk" : configName).Bind(o);
                configureOptions?.Invoke(o);
            });

        return new AgonesSdkBuilder(services);
    }

    public static IAgonesSdkBuilder UseHostedService(this IAgonesSdkBuilder builder)
    {
        // Add HostedService
        builder.Services.AddHostedService<AgonesHostedService>();
        return builder;
    }
}
