using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevServer;
using Shouldly;
using Xunit;

namespace Tests;

public class IntegrationTests
{
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

        var context     = new HostedServiceContext();
        var hostBuilder = Program.CreateHostBuilder(Array.Empty<string>(), context);
        var host        = hostBuilder.Build();

        await host.StartAsync();

        //await Task.Delay(5000); // TODO get signal from host.

        var client = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{context.LoadBalancer.Port}")
        };

        var response = await client.GetAsync("/remote");

        await host.StopAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}