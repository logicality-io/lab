using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Logicality.Extensions.Hosting;

namespace Logicality.ExampleGateway.DevServer;

public class RedisHostedService : DockerHostedService
{
    private readonly HostedServiceContext _context;

    public RedisHostedService(
        HostedServiceContext context,
        ILogger<DockerHostedService> logger,
        bool leaveRunning = false)
        : base(logger, leaveRunning)
    {
        _context = context;
    }

    public int Port { get; } = 6379;

    protected override IContainerService CreateContainerService()
        => new Builder()
            .UseContainer()
            .WithName(ContainerName)
            .UseImage("redis:6-alpine")
            .ReuseIfExists()
            .ExposePort(Port, 6379)
            .Build();

    protected override string ContainerName => "ExampleGatewayRedis";

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);

        _context.Redis = this;
    }
}