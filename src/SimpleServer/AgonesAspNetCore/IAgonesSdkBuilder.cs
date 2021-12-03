namespace SimpleServer.AgonesAspNetCore;

public interface IAgonesSdkBuilder
{
    IServiceCollection Services { get; }
}

internal class AgonesSdkBuilder : IAgonesSdkBuilder
{
    public IServiceCollection Services { get; }
    public AgonesSdkBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentException(nameof(services));
    }
}
