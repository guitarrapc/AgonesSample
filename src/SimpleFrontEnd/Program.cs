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
builder.Services.AddSingleton<IAgonesAllocationDatabase, InmemoryAgonesAllocationDatabase>();
builder.Services.AddSingleton<KubernetesApiService>();
builder.Services.AddSingleton<AgonesAllocationService>();
builder.Services.AddSingleton<AgonesGameServerService>();
builder.Services.AddSingleton<AgonesServerRpcService>();

builder.Services.AddHttpClient("kubernetes-api")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
    });

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
