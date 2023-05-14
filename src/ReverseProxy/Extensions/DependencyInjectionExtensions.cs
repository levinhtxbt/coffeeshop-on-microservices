using Yarp.ReverseProxy.Configuration;

namespace ReverseProxy.Extensions;

public static class DependencyInjectionExtensions
{
    public static IReverseProxyBuilder LoadFromDiscoveryService(this IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<InMemoryConfigProvider>();

        builder.Services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        builder.Services.AddSingleton<IProxyConfigProvider>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        return builder;
    }
}