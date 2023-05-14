using System.Net;
using N8T.Infrastructure.OTel;
using ReverseProxy.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs()
    .ConfigureKestrel(webBuilder =>
    {
        webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 5000)); // REST
    });

builder.Configuration.AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .LoadFromDiscoveryService();

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();
app.Run();