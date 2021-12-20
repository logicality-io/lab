using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Logicality.Extensions.Hosting;

namespace DevServer;

public class SeqHostedService : DockerHostedService
{
    private readonly HostedServiceContext _context;
    private const    int                  HostPort = 5110;
    private const    int                  ContainerPort   = 80;

    public SeqHostedService(
        HostedServiceContext         context,
        ILogger<DockerHostedService> logger)
        : base(logger)
    {
        _context  = context;
        SinkUri   = new Uri($"http://localhost:{HostPort}");
    }

    protected override string ContainerName => "ExampleGatewaySeq";

    public Uri SinkUri { get; }

    protected override IContainerService CreateContainerService()
        => new Builder()
            .UseContainer()
            .WithName(ContainerName)
            .UseImage("datalust/seq:latest")
            .ReuseIfExists()
            .ExposePort(HostPort, ContainerPort)
            .WithEnvironment("ACCEPT_EULA=Y")
            .WaitForPort($"{ContainerPort}/tcp", 5000, "127.0.0.1")
            .Build();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        _context.Seq = this;
    }
}