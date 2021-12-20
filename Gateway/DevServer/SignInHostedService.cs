using Logicality.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Serilog;
using Signin;

namespace DevServer;

public class SignInHostedService : IHostedService
{
    private readonly HostedServiceContext _context;
    private          IWebHost?            _webHost;

    public SignInHostedService(HostedServiceContext context)
    {
        _context = context;
    }

    public int Port { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("RedisEndpoint", $"localhost:{_context.Redis.Port}"),
            })
            .Build();
        _webHost = WebHost.CreateDefaultBuilder<Startup>(Array.Empty<string>())
            .UseUrls($"https://+:{Port}")
            .UseConfiguration(config)
            .UseSerilog()
            .Build();

        await _webHost.StartAsync(cancellationToken);

        Port = _webHost.GetServerUris()[0].Port;

        Log.Logger.Information("{name} started on port: {port}", nameof(SignInHostedService), Port);

        _context.SignIn = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}

