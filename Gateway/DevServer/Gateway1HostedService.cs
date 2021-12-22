using Gateway;
using Logicality.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace Logicality.ExampleGateway.DevServer;

public class Gateway1HostedService : IHostedService
{
    private readonly HostedServiceContext _context;
    private IWebHost? _webHost;
    private const int DefaultPort = 5001;

    public Gateway1HostedService(HostedServiceContext context)
    {
        _context = context;
        Port = context.FixedPorts ? DefaultPort : 0;
    }

    public int Port { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                //{ "SeqUri", _context.Seq.SinkUri.ToString() }
            })
            .Build();
        _webHost = WebHost
            .CreateDefaultBuilder<Startup>(Array.Empty<string>())
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .ConfigureLogging(l => _context.ConfigureLogging(l))
            .Build();

        await _webHost.StartAsync(cancellationToken);

        Port = _webHost.GetServerUris()[0].Port;

        _context.Gateway1 = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}
