using AgonesAspNetCore;
using SimpleBackendGrpc.Infrastructures;
using SimpleBackendGrpc.Services;

namespace SimpleBackendGrpc;

public class Program
{
    public static async Task Main(string[] args)
    {
        var option = new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = Path.GetDirectoryName(typeof(Program).Assembly.Location),
        };
        var builder = WebApplication.CreateBuilder(option);
        builder.Configuration.AddJsonFile($"appsettings.Grpc.json");
        builder.Configuration.AddJsonFile($"appsettings.Grpc.{builder.Environment.EnvironmentName}.json", optional: true);

        // Add AgonesSDK
        builder.AddAgonesService().EnableHealthCheck();

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddMagicOnion();

        // Configure Server Listener
        builder.ConfigureEndpoint();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GreeterService>();
        app.MapMagicOnionService();

        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        await app.RunAsync();
    }
}
