﻿using Gateway;
using Microsoft.AspNetCore;

namespace DevServer;

public class Gateway2HostedService : IHostedService
{
    private readonly HostedServiceContext _context;
    private          IWebHost?            _webHost;
    public const     int                  Port = 5001;

    public Gateway2HostedService(HostedServiceContext context)
    {
        _context = context;
    }

    public Task StartAsync(CancellationToken cancellationToken)
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
            .Build();

        return _webHost.StartAsync(cancellationToken);
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}
