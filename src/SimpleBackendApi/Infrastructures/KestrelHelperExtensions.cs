namespace SimpleBackendApi.Infrastructures;

public static class KestrelHelperExtensions
{
    private static readonly string defaultListenAddress = "http://0.0.0.0:5012";

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
        Console.WriteLine($"Listener Address: {endPoint.Address}:{endPoint.Port}");
    }
}
