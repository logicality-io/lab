using Gateway;
using Logicality.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;

namespace DevServer;

public class Gateway1HostedService : IHostedService
{
    private readonly HostedServiceContext _context;
    private          IWebHost?            _webHost;

    public Gateway1HostedService(HostedServiceContext context)
    {
        _context = context;
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
            .UseUrls($"http://+:0")
            .UseConfiguration(config)
            .UseSerilog()
            .Build();

        await _webHost.StartAsync(cancellationToken);

        Port = _webHost.GetServerUris()[0].Port;

        _context.Gateway1 = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}
