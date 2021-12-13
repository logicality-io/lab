using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Logicality.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevServer;

public class RedisHostedService : DockerHostedService
{
    private readonly HostedServiceContext _context;
    private const    int                  HostPort = 6379;
            
    public RedisHostedService(
        HostedServiceContext         context, 
        ILogger<DockerHostedService> logger,
        bool                         leaveRunning = false) 
        : base(logger, leaveRunning)
    {
        _context = context;
    }

    protected override IContainerService CreateContainerService()
        => new Builder()
            .UseContainer()
            .WithName(ContainerName)
            .UseImage("redis:6-alpine")
            .ReuseIfExists()
            .ExposePort(HostPort, HostPort)
            .Build();

    protected override string ContainerName => "ExampleGatewayRedis";

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _context.Redis = this;
    }
}