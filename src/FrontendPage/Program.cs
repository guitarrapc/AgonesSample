using FrontendPage.Clients;
using FrontendPage.Infrastructures;
using FrontendPage.Services;
using Microsoft.Net.Http.Headers;
using Shared;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Application services to the container.
builder.RegisterApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<FrontendPage.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

public static class StartupExtentions
{
    public static void RegisterApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAgonesAllocationDatabase, InmemoryAgonesAllocationDatabase>();
        builder.Services.AddSingleton<AgonesMagicOnionClient>();
        builder.Services.AddSingleton<AgonesAllocationService>();
        builder.Services.AddSingleton<AgonesGameServerService>();

        builder.Services.AddSingleton<KubernetesApiClient>();
        builder.Services.AddHttpClient("kubernetes-api", httpClient =>
        {
            httpClient.BaseAddress = new Uri(KubernetesServiceProvider.Current.KubernetesServiceEndPoint);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", KubernetesServiceProvider.Current.AccessToken);
        })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        builder.Services.AddSingleton<AgonesApiClient>();
        builder.Services.AddHttpClient("agnoes-api", httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });
    }
}
