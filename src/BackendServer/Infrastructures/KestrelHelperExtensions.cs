using Shared;

namespace BackendServerGrpc.Infrastructures;

public static class KestrelHelperExtensions
{
    private static readonly string defaultListenAddress = DockerServiceProvider.Current.IsRunningOnDocker ? "http://0.0.0.0:8080" : "http://0.0.0.0:5157";
    private static readonly string defaultProtocol = "h2c";

    public static void ConfigureEndpoint(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var endpoint = CreateIPEndpoint(context.Configuration);
            options.Listen(endpoint.Address, endpoint.Port, listenOptions =>
            {
                ConfigureListenOptions(listenOptions, context.Configuration, endpoint);
            });

            // Other languages gRPC server don't include a server header
            options.AddServerHeader = false;
        });
    }

    private static System.Net.IPEndPoint CreateIPEndpoint(IConfiguration config)
    {
        var address = CreateBindingAddress(config);

        // listen on `127.0.0.1` or `0.0.0.0` or `[::]`, won't listen on server name.
        System.Net.IPAddress? ip;
        if (string.Equals(address.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            ip = System.Net.IPAddress.Loopback;
        }
        else if (!System.Net.IPAddress.TryParse(address.Host, out ip))
        {
            ip = System.Net.IPAddress.IPv6Any;
        }

        return new System.Net.IPEndPoint(ip, address.Port);

        static BindingAddress CreateBindingAddress(IConfiguration config)
        {
            var url = config.GetValue<string>("Url") ?? defaultListenAddress;
            return BindingAddress.Parse(url);
        }
    }

    private static void ConfigureListenOptions(Microsoft.AspNetCore.Server.Kestrel.Core.ListenOptions listenOptions, IConfiguration config, System.Net.IPEndPoint endPoint)
    {
        // default is Insecure gRPC
        var protocol = config.GetValue<string>("Protocol") ?? defaultProtocol;
        var useClientAuth = config.GetValue<bool?>("ClientAuth") ?? false;

        Console.WriteLine($"Listener Address: {endPoint.Address}:{endPoint.Port}, Protocol: {protocol}");

        switch (protocol)
        {
            case "h2c":
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                    break;
                }
            default:
                throw new NotImplementedException(protocol);
        }
    }
}
