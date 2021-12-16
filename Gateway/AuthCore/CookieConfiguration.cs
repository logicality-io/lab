using Microsoft.AspNetCore.Http;

namespace AuthCore;

public static class CookieConfiguration
{
    public const string       CookieName   = "axiom.cookie";
    public const SameSiteMode SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
}