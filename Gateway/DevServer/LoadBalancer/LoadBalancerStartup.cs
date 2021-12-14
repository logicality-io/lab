using Yarp.ReverseProxy.Configuration;

namespace DevServer.LoadBalancer
{
    public class LoadBalancerStartup
    {
        private readonly IConfiguration      _configuration;
        private readonly IWebHostEnvironment _environment;

        public LoadBalancerStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment   = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddReverseProxy()
                .LoadFromConfig(_configuration.GetSection("ReverseProxy"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });
        }
    }
}
