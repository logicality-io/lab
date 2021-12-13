using Microsoft.AspNetCore;

namespace DevServer;

public class LoadBalancerHostedService : IHostedService
{
    private readonly HostedServiceContext _context;
    private          IWebHost?            _webHost;
    public const     int                  Port = 5000;

    public LoadBalancerHostedService(HostedServiceContext context)
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

    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
