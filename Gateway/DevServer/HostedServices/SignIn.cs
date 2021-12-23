using Logicality.AspNetCore.Hosting;
using Logicality.ExampleGateway.SignIn;

using Microsoft.AspNetCore;

namespace Logicality.ExampleGateway.DevServer.HostedServices;

public class SignIn : IHostedService
{
    private readonly HostedServiceContext _context;
    private readonly ILogger<SignIn>      _logger;
    private          IWebHost?            _webHost;
    private const    int                  DefaultPort = 5003;

    public SignIn(HostedServiceContext context, ILogger<SignIn> logger)
    {
        _context     = context;
        _logger = logger;
        Port = context.FixedPorts ? DefaultPort : 0;
    }

    public int Port { get; set; }

    public string Address { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RedisEndpoint", $"localhost:{_context.Redis.Port}" }
            })
            .Build();
        _webHost = WebHost.CreateDefaultBuilder<Startup>(Array.Empty<string>())
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                _context.ConfigureLogging(loggingBuilder);
            })
            .Build();

        await _webHost.StartAsync(cancellationToken);

        Port    = _webHost.GetServerUris()[0].Port;
        Address = $"http://signin.app.all-localhost.com:{Port}/Account/Login";

        _logger.LogInformation("{name} running on: {address}", nameof(SignIn), Address);

        _context.SignIn = this;
    }

    public Task? StopAsync(CancellationToken cancellationToken)
        => _webHost?.StopAsync(cancellationToken);
}

