using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Logicality.ExampleGateway.DevServer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class IntegrationTests
{
    private readonly ITestOutputHelper _outputHelper;

    public IntegrationTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Can_make_authenticated_request()
    {
        /* 1. Request
         2. Gateway redirects to signin
         3. Initiate sigin which redirects to Idp.
         4. Auth with Idp
         5. Redirect back to signin, get cookie
         6. Redirect to original request on gateway.
        TODO: sequence diagram.
        */

        void ConfigureLogging(ILoggingBuilder l) => l.AddXUnit(_outputHelper);
        var context     = new HostedServiceContext(ConfigureLogging, fixedPorts: false);
        var hostBuilder = Program
            .CreateHostBuilder(Array.Empty<string>(), context)
            .ConfigureLogging(l => l.AddXUnit(_outputHelper));
        var host = hostBuilder.Build();

        await host.StartAsync();

        var client = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{context.LoadBalancer.Port}")
        };

        var response = await client.GetAsync("/remote");

        await host.StopAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}