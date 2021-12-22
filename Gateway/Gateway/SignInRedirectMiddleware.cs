using System;
using System.Threading.Tasks;
using Logicality.ExampleGateway.AuthCookieHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Logicality.ExampleGateway.Gateway;

public class SignInRedirectMiddleware
{
    private readonly RequestDelegate                   _next;
    private readonly ILogger<SignInRedirectMiddleware> _logger;
    private readonly GatewayOptions                    _options;

    public SignInRedirectMiddleware(
        RequestDelegate next,
        ILogger<SignInRedirectMiddleware> logger,
        IOptions<GatewayOptions> options)
    {
        _next    = next;
        _logger  = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cookieExists = context.Request.Cookies.TryGetValue(CookieConfiguration.CookieName, out _);
        _logger.LogInformation($"Auth Cookie Exists: {cookieExists}");
        if (!cookieExists)
        {
            var returnUrl = context.Request.GetEncodedUrl();

            var uriBuilder = new UriBuilder(new Uri(_options.SignInAddress))
            {
                Query = $"ReturnUrl={returnUrl}"
            };
            // $"https://localhost:5003/Account/Login?ReturnUrl={returnUrl}"
            var redirectAddress = uriBuilder.Uri.ToString();
            context.Response.Redirect(redirectAddress);
            return;
        }

        await _next(context);
    }
}