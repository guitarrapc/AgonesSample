using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SimpleFrontEnd.Data;
using SimpleFrontEnd.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Add Application services to the container.
builder.RegisterApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


public static class StartupExtentions
{
    public static void RegisterApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAgonesAllocationDatabase, InmemoryAgonesAllocationDatabase>();
        builder.Services.AddSingleton<AgonesAllocatorApiClient>();
        builder.Services.AddSingleton<BackendServerRpcClient>();
        builder.Services.AddSingleton<KubernetesApiClient>();
        builder.Services.AddSingleton<AgonesAllocationService>();
        builder.Services.AddSingleton<AgonesGameServerService>();

        builder.Services.AddHttpClient("kubernetes-api")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        builder.Services.AddHttpClient("agonesallocator-api")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });
    }
}
