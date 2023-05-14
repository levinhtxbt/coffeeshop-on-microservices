using Microsoft.Extensions.Primitives;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Eureka;
using Yarp.ReverseProxy.Configuration;

namespace ReverseProxy.Extensions;

public class InMemoryConfigProvider : IProxyConfigProvider, IHostedService, IDisposable
{
    private Timer _timer;
    private volatile InMemoryConfig _config;
    private readonly DiscoveryClient _discoveryClient;
    private readonly RouteConfig[] _routes;

    public InMemoryConfigProvider(IDiscoveryClient discoveryClient, RouteConfig[] routes = null)
    {
        _discoveryClient = discoveryClient as DiscoveryClient;
        // foreach (var section in _configuration.GetSection("Clusters").GetChildren())
        // {
        //     newSnapshot.Clusters.Add(CreateCluster(section));
        // }
        //var routes = proxyConfig.Routes;
        // TODO: Read from appsettings
        //_routes = routes.ToArray();
        _routes = routes;
        
        PopulateConfig();
    }

    private void Update(object state)
    {
        PopulateConfig();
    }

    private void PopulateConfig()
    {
        var apps = _discoveryClient.Applications.GetRegisteredApplications();
        List<ClusterConfig> clusters = new();

        foreach (var app in apps)
        {
            var cluster = new ClusterConfig
            {
                ClusterId = app.Name.ToLower(),
                Destinations = app.Instances
                .Select(x =>
                    (x.InstanceId,
                        new DestinationConfig()
                        {
                            Address = $"http://{x.HostName.ToLower()}:{x.Port}"
                        }))
                .ToDictionary(y => y.InstanceId, y => y.Item2)
            };

            clusters.Add(cluster);
        }

        var oldConfig = _config;
        _config = new InMemoryConfig(_routes, clusters);
        oldConfig?.SignalChange();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }


    // private ClusterConfig CreateCluster(IConfigurationSection section)
    // {
    //     var destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);
    //     foreach (var destination in section.GetSection(nameof(ClusterConfig.Destinations)).GetChildren())
    //     {
    //         destinations.Add(destination.Key, CreateDestination(destination));
    //     }

    //     return new ClusterConfig
    //     {
    //         ClusterId = section.Key,
    //         LoadBalancingPolicy = section[nameof(ClusterConfig.LoadBalancingPolicy)],
    //         SessionAffinity = CreateSessionAffinityConfig(section.GetSection(nameof(ClusterConfig.SessionAffinity))),
    //         HealthCheck = CreateHealthCheckConfig(section.GetSection(nameof(ClusterConfig.HealthCheck))),
    //         HttpClient = CreateHttpClientConfig(section.GetSection(nameof(ClusterConfig.HttpClient))),
    //         HttpRequest = CreateProxyRequestConfig(section.GetSection(nameof(ClusterConfig.HttpRequest))),
    //         Metadata = section.GetSection(nameof(ClusterConfig.Metadata)).ReadStringDictionary(),
    //         Destinations = destinations,
    //     };
    // }

    // private static HealthCheckConfig? CreateHealthCheckConfig(IConfigurationSection section)
    // {
    //     if (!section.Exists())
    //     {
    //         return null;
    //     }

    //     return new HealthCheckConfig
    //     {
    //         Passive = CreatePassiveHealthCheckConfig(section.GetSection(nameof(HealthCheckConfig.Passive))),
    //         Active = CreateActiveHealthCheckConfig(section.GetSection(nameof(HealthCheckConfig.Active))),
    //         AvailableDestinationsPolicy = section[nameof(HealthCheckConfig.AvailableDestinationsPolicy)]
    //     };
    // }


    // private static SessionAffinityConfig? CreateSessionAffinityConfig(IConfigurationSection section)
    // {
    //     if (!section.Exists())
    //     {
    //         return null;
    //     }

    //     return new SessionAffinityConfig
    //     {
    //         Enabled = section.ReadBool(nameof(SessionAffinityConfig.Enabled)),
    //         Policy = section[nameof(SessionAffinityConfig.Policy)],
    //         FailurePolicy = section[nameof(SessionAffinityConfig.FailurePolicy)],
    //         AffinityKeyName = section[nameof(SessionAffinityConfig.AffinityKeyName)]!,
    //         Cookie = CreateSessionAffinityCookieConfig(section.GetSection(nameof(SessionAffinityConfig.Cookie)))
    //     };
    // }

    // private static DestinationConfig CreateDestination(IConfigurationSection section)
    // {
    //     return new DestinationConfig
    //     {
    //         Address = section[nameof(DestinationConfig.Address)]!,
    //         Health = section[nameof(DestinationConfig.Health)],
    //         Metadata = section.GetSection(nameof(DestinationConfig.Metadata)).ReadStringDictionary(),
    //     };
    // }

    public IProxyConfig GetConfig() => _config;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }


    private class InMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }


        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }

    }

}