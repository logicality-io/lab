using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class IntegrationTests
{
    [Fact]
    public async Task Test1()
    {
        var hostBuilder = DevServer.Program.CreateHostBuilder(Array.Empty<string>());

        var cancellationTokenSource = new CancellationTokenSource();

        var host = hostBuilder.Build().StartAsync(cancellationTokenSource.Token);

        await Task.Delay(1000);

        cancellationTokenSource.Cancel();

        await host;
    }
}