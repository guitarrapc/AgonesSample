using Agones;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleBackendGrpc.AgonesAspNetCore;
using SimpleShared;

namespace AgonesAspNetCore;

public interface IAgonesServiceBuilder
{
    IServiceCollection Services { get; }
}

internal class AgonesServiceBuilder : IAgonesServiceBuilder
{
    public IServiceCollection Services { get; }
    public AgonesServiceBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentException(nameof(services));
    }
}

public static class AgoneServiceExtensions
{
    /// <summary>
    /// Register AgonesSDK
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IAgonesServiceBuilder AddAgonesService(this WebApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<AgonesCondition>();
        // Configuration
        builder.Services.Configure<AgonesOptions>(builder.Configuration.GetSection("AgonesOptions"));

        if (KubernetesServiceProvider.Current.IsRunningOnKubernetes)
        {
            builder.Services.TryAddSingleton<IAgonesSDK, AgonesSDK>();
        }
        else
        {
            builder.Services.TryAddSingleton<IAgonesSDK, AgonesSDKPresudo>();
            builder.Services.AddHostedService<AgonesStartupService>();
        }

        return new AgonesServiceBuilder(builder.Services);
    }

    /// <summary>
    /// Enable Agones HealthCheck Background Service
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IAgonesServiceBuilder EnableHealthCheck(this IAgonesServiceBuilder builder)
    {
        builder.Services.AddHostedService<AgonesHealthCheckService>();
        return builder;
    }
}
