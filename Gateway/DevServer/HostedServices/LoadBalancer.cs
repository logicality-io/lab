using Microsoft.AspNetCore;
using Yarp.ReverseProxy.Configuration;

namespace Logicality.ExampleGateway.DevServer.HostedServices;

public class LoadBalancer : IHostedService
{
    private const    int                   DefaultPort = 5000;
    private readonly HostedServiceContext  _context;
    private readonly ILogger<LoadBalancer> _logger;
    private          IWebHost?             _webHost;

    public LoadBalancer(HostedServiceContext context, ILogger<LoadBalancer> logger)
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
            Match = new RouteMatch { Path = "{**catch-all}" }
        };
        routes.Add("route1", routeConfig);

        var clusters = new Dictionary<string, ClusterConfig>();
        var clusterConfig = new ClusterConfig
        {
            ClusterId = "gateway",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "gateway-1", new DestinationConfig { Address = _context.Gateway1.Address} },
                { "gateway-2", new DestinationConfig { Address = _context.Gateway2.Address} }
            }
        };
        clusters.Add("gateway", clusterConfig);

        var proxyConfig = new ProxyConfig(routes, clusters);

        var config = new ConfigurationBuilder()
            .AddObject(proxyConfig)
            .Build();
        _webHost = WebHost
            .CreateDefaultBuilder<LoadBalancerStartup>(Array.Empty<string>())
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                _context.ConfigureLogging(loggingBuilder);
            })
            .Build();

        await _webHost.StartAsync(cancellationToken);

        _logger.LogInformation($"Load Balancer Running on http://app.all-localhost.com:{Port}");

        _context.LoadBalancer = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);

    public class LoadBalancerStartup
    {
        private readonly IConfiguration      _configuration;
        private readonly IWebHostEnvironment _environment;

        public LoadBalancerStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment   = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddReverseProxy()
                .LoadFromConfig(_configuration.GetSection("ReverseProxy"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });
        }
    }

    internal class ProxyConfig
    {
        public ProxyConfig(
            Dictionary<string, RouteConfig> routes,
            Dictionary<string, ClusterConfig> clusters)
        {
            ReverseProxy = new Data
            {
                Routes = routes,
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
