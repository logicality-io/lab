using Logicality.ExampleGateway.DevServer.HostedServices;
using Logicality.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Logicality.ExampleGateway.DevServer;

public class Program
{
    internal static async Task Main(string[] args)
    {
        var programLogger = CreateLogConfiguration()
            .WriteTo.Logger(l => l.WriteHostedServiceMessagesToConsole())
            .WriteTo.Seq($"http://localhost:{Seq.HostPort}")
            .CreateLogger();

        void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            var logger = CreateLogConfiguration()
                .WriteTo.Seq($"http://localhost:{Seq.HostPort}")
                .CreateLogger();
            loggingBuilder.AddSerilog(logger);
        }

        var context = new HostedServiceContext(ConfigureLogging);
        
        var host = CreateHostBuilder(args, context)
            .UseSerilog(programLogger)
            .Build();

        await host.RunAsync();
    }

    private static LoggerConfiguration CreateLogConfiguration() =>
        new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext();

    public static IHostBuilder CreateHostBuilder(string[] args, HostedServiceContext context) =>
        new HostBuilder()
            .UseConsoleLifetime()
            .ConfigureServices(services =>
            {
                services.AddSingleton(context);
                services.AddTransient<Seq>();
                services.AddTransient<Redis>();
                services.AddTransient<LoadBalancer>();
                services.AddTransient<HostedServices.IdentityProvider>();
                services.AddTransient<Gateway1>();
                services.AddTransient<Gateway2>();
                services.AddTransient<SignIn>();

                services
                    .AddSequentialHostedServices("root", r => r
                        .Host<HostedServices.Seq>()
                        .HostParallel("services", p => p
                            .Host<HostedServices.IdentityProvider>()
                            .Host<Redis>()
                        )
                        .Host<SignIn>()
                        .HostParallel("gateways", g => g
                            .Host<Gateway1>()
                            .Host<Gateway2>()
                        )
                        .Host<LoadBalancer>());
            });
}