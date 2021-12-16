using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Logicality.Extensions.Hosting;

namespace DevServer;

public class RedisHostedService : DockerHostedService
{
    private const    int                  DefaultHostPort = 6379;
    private readonly HostedServiceContext _context;
    private readonly int                  _hostPort;
            
    public RedisHostedService(
        HostedServiceContext         context, 
        ILogger<DockerHostedService> logger,
        bool                         leaveRunning = false) 
        : base(logger, leaveRunning)
    {
        _context = context;
        _hostPort = context.FixedPorts ? DefaultHostPort : 0;
    }

    protected override IContainerService CreateContainerService()
        => new Builder()
            .UseContainer()
            .WithName(ContainerName)
            .UseImage("redis:6-alpine")
            .ReuseIfExists()
            .ExposePort(_hostPort, DefaultHostPort)
            .Build();

    protected override string ContainerName => "ExampleGatewayRedis";

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _context.Redis = this;
    }
}