using Steeltoe.Discovery;
using Yarp.ReverseProxy.Configuration;

namespace ReverseProxy.Extensions;

public static class DependencyInjectionExtensions
{
    public static IReverseProxyBuilder LoadFromDiscoveryService(this IReverseProxyBuilder builder)
    {
        var routeConfig = builder.Services.BuildServiceProvider().GetRequiredService<IProxyConfigProvider>().GetConfig();
        // Reuse Routes config
        builder.Services.AddSingleton<InMemoryConfigProvider>(ctx => 
            new InMemoryConfigProvider(ctx.GetRequiredService<IDiscoveryClient>(), routeConfig.Routes.ToArray()));

        builder.Services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        builder.Services.AddSingleton<IProxyConfigProvider>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        return builder;
    }
}