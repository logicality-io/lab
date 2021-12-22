using Logicality.AspNetCore.Hosting;
using Logicality.ExampleGateway.Gateway;
using Microsoft.AspNetCore;

namespace Logicality.ExampleGateway.DevServer.HostedServices;

public class Gateway2 : IHostedService
{
    private readonly HostedServiceContext           _context;
    private readonly ILogger<Gateway2> _logger;
    private          IWebHost?                      _webHost;
    private const    int                            DefaultPort = 5002;

    public Gateway2(HostedServiceContext context, ILogger<Gateway2> logger)
    {
        _context = context;
        _logger  = logger;
        Port     = context.FixedPorts ? DefaultPort : 0;
    }

    public int Port { get; set; }

    public string Address { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "SignInAddress", _context.SignIn.Address }
            })
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
        Address = $"http://gw2.app-int.all-localhost.com:{Port}";

        _logger.LogInformation($"Gateway 2 running on {Address}");

        _context.Gateway2 = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}
