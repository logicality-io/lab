using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Duende.Bff;
using Duende.Bff.Yarp;
using Logicality.ExampleGateway.AuthCookieHandling;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Gateway;

public class Startup
{
    private readonly IConfiguration      _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment   = environment;

        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBff()
            .AddRemoteApis();

        services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostTicketConfiguration>();
        services.AddTransient<ISessionRevocationService, SessionRevocationService>();

        services.AddSingleton<IDataProtectionProvider, InsecureDataProtectionProvider>();
        services.AddTransient<ITicketStore, RedisCacheTicketStore>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
            })
            .AddCookie("cookie", options =>
            {
                options.Cookie.Name     = CookieConfiguration.CookieName;
                options.Cookie.SameSite = CookieConfiguration.SameSiteMode;
                options.Cookie.HttpOnly = true;
            });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseMiddleware<SignInRedirectMiddleware>();

        app.UseAuthentication();
        app.UseBff();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRemoteBffApiEndpoint(
                    "/remote", //"/profile",
                    "https://demo.duendesoftware.com/api/test", /* "https://localhost:5002/profile", */
                    false)
                .RequireAccessToken();
        });
    }
}