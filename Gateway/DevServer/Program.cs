using DevServer;
using Logicality.Extensions.Hosting;
using Serilog;
using Serilog.Events;

await CreateHostBuilder(args)
    .Build()
    .RunAsync();

static IHostBuilder CreateHostBuilder(string[] args)
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

    var context = new HostedServiceContext();

    return new HostBuilder()
        .UseConsoleLifetime()
        .ConfigureServices(services =>
        {
            services.AddSingleton(context);
            services.AddTransient<RedisHostedService>();
            services.AddTransient<LoadBalancerHostedService>();
            services.AddTransient<Gateway1HostedService>();
            services.AddTransient<Gateway2HostedService>();

            services.AddSequentialHostedServices("root", r => r
                .Host<RedisHostedService>()
                .Host<Gateway1HostedService>());
            /*.Host<Gateway2HostedService>()
            .Host<LoadBalancerHostedService>());*/

        })
        .UseSerilog(logger);
}