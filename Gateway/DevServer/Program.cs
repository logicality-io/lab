using Logicality.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DevServer;

public class Program
{
    internal static async Task Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Logger(l =>
            {
                l.WriteHostedServiceMessagesToConsole();
            });

        var logger = loggerConfiguration.CreateLogger();

        void ConfigureLogging(ILoggingBuilder l) => l.AddSerilog(logger);
        var context = new HostedServiceContext(ConfigureLogging);
        var host = CreateHostBuilder(args, context).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args, HostedServiceContext context) =>
        new HostBuilder()
            .UseConsoleLifetime()
            .ConfigureServices(services =>
            {
                services.AddSingleton(context);
                services.AddTransient<RedisHostedService>();
                services.AddTransient<GatewayLoadBalancerHostedService>();
                services.AddTransient<Gateway1HostedService>();
                services.AddTransient<Gateway2HostedService>();

                services
                    .AddSequentialHostedServices("root",
                        r =>
                        {
                            r.HostParallel("gateways", p =>
                            {
                                p.Host<SeqHostedService>()
                                    .Host<RedisHostedService>()
                                    .Host<Gateway1HostedService>()
                                    .Host<Gateway2HostedService>();
                            });
                            r.Host<GatewayLoadBalancerHostedService>();
                        });
            });
}