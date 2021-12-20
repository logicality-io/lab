using DevServer.LoadBalancer;
using Logicality.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Yarp.ReverseProxy.Configuration;

namespace DevServer;

public class GatewayLoadBalancerHostedService : IHostedService
{
    private const    int                                       DefaultPort = 5000;
    private readonly HostedServiceContext                      _context;
    private readonly ILogger<GatewayLoadBalancerHostedService> _logger;
    private          IWebHost?                                 _webHost;

    public GatewayLoadBalancerHostedService(HostedServiceContext context, ILogger<GatewayLoadBalancerHostedService> logger)
    {
        _context = context;
        _logger  = logger;
        Port     = context.FixedPorts ? DefaultPort : 0;
    }

    public int Port { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var routes = new Dictionary<string, RouteConfig>();
        var routeConfig = new RouteConfig
        {
            ClusterId = "gateway",
            Match     = new RouteMatch { Path = "{**catch-all}" }
        };
        routes.Add("route1", routeConfig);

        var clusters = new Dictionary<string, ClusterConfig>();
        var clusterConfig = new ClusterConfig
        {
            ClusterId = "gateway",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "gateway-1", new DestinationConfig { Address = $"http://gw1.int.all-localhost.com:{_context.Gateway1.Port}" } },
                { "gateway-2", new DestinationConfig { Address = $"http://gw2.int.all-localhost.com:{_context.Gateway2.Port}" } }
            }
        };
        clusters.Add("gateway", clusterConfig);

        var proxyConfig = new ProxyConfig(routes, clusters);

        /*var json = JsonSerializer.Serialize(proxyConfig,
            new JsonSerializerOptions{ DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });*/

        var config = new ConfigurationBuilder()
            .AddObject(proxyConfig)
            .Build();
        _webHost = WebHost
            .CreateDefaultBuilder<LoadBalancerStartup>(Array.Empty<string>())
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .Build();

        await _webHost.StartAsync(cancellationToken);

        _context.LoadBalancer = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);

    internal class ProxyConfig
    {
        public ProxyConfig(
            Dictionary<string, RouteConfig> routes,
            Dictionary<string, ClusterConfig> clusters)
        {
            ReverseProxy = new Data
            {
                Routes   = routes,
                Clusters = clusters
            };
        }

        public Data ReverseProxy { get; set; }

        public class Data
        {
            public Dictionary<string, RouteConfig> Routes { get; set; }

            public Dictionary<string, ClusterConfig> Clusters { get; set; }
        }
    }
}
