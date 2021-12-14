using System;
using System.Net.Http;
using System.Threading.Tasks;
using DevServer;
using Xunit;

namespace Tests;

public class IntegrationTests
{
    [Fact]
    public async Task Test1()
    {
        var context     = new HostedServiceContext();
        var hostBuilder = DevServer.Program.CreateHostBuilder(Array.Empty<string>(), context);

        var host = hostBuilder.Build();

        await host.StartAsync();

        await Task.Delay(2000); // TODO get signal from host.

        var client = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{context.LoadBalancer.Port}")
        };

        var response = await client.GetAsync("/todos");

        await host.StopAsync();
    }
}