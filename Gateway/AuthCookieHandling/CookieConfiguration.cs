using Microsoft.AspNetCore.Http;

namespace Logicality.ExampleGateway.AuthCookieHandling;

public static class CookieConfiguration
{
    public const string       CookieName   = "_auth";
    public const SameSiteMode SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
}