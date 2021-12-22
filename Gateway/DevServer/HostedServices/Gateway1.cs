using Logicality.AspNetCore.Hosting;
using Logicality.ExampleGateway.Gateway;
using Microsoft.AspNetCore;

namespace Logicality.ExampleGateway.DevServer.HostedServices;

public class Gateway1 : IHostedService
{
    private readonly HostedServiceContext           _context;
    private readonly ILogger<Gateway1> _logger;
    private          IWebHost?                      _webHost;
    private const    int                            DefaultPort = 5001;

    public Gateway1(HostedServiceContext context, ILogger<Gateway1> logger)
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
        Address = $"http://gw1.app-int.all-localhost.com:{Port}";

        _logger.LogInformation($"Gateway 1 running on {Address}");

        _context.Gateway1 = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}
