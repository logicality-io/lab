using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Gateway;

public class SignInRedirectMiddleware
{
    private readonly RequestDelegate                   _next;
    private readonly ILogger<SignInRedirectMiddleware> _logger;

    public SignInRedirectMiddleware(RequestDelegate next, ILogger<SignInRedirectMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cookieExists = context.Request.Cookies.TryGetValue("axiom.cookie", out _);
        _logger.LogInformation($"BFF Cookie Exists: {cookieExists}");
        if (!cookieExists)
        {
            var returnUrl = context.Request.GetEncodedUrl();
            context.Response.Redirect($"https://localhost:5003/Account/Login?ReturnUrl={returnUrl}");
            return;
        }

        await _next(context);
    }
}