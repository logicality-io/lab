using IdentityProvider;
using Logicality.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Startup = Logicality.ExampleGateway.Gateway.Startup;

namespace Logicality.ExampleGateway.DevServer.HostedServices;

public class IdentityProvider : IHostedService
{
    private readonly HostedServiceContext      _context;
    private readonly ILogger<IdentityProvider> _logger;
    private          IWebHost?                 _webHost;
    private const    int                       DefaultPort = 5050;

    public IdentityProvider(HostedServiceContext context, ILogger<IdentityProvider> logger)
    {
        _context     = context;
        _logger = logger;

        Port = context.FixedPorts ? DefaultPort : 0;
    }

    public int Port { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        _webHost = WebHost
            .CreateDefaultBuilder<Startup>(Array.Empty<string>())
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                _context.ConfigureLogging(loggingBuilder);
            })
            .Build();

        await _webHost.StartAsync(cancellationToken);

        Port = _webHost.GetServerUris()[0].Port;

        _logger.LogInformation($"Identity Provider Running on http://idp.all-localhost.com:{Port}");

        _context.IdentityProvider = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}

