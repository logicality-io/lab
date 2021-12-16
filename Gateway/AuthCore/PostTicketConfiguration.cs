using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace AuthCore;

public class PostTicketConfiguration : IPostConfigureOptions<CookieAuthenticationOptions>
{
    private readonly string? _scheme;

    public PostTicketConfiguration(IOptions<AuthenticationOptions> options)
    {
        _scheme = options.Value.DefaultAuthenticateScheme ?? options.Value.DefaultScheme;
    }
    public void PostConfigure(string name, CookieAuthenticationOptions options)
    {
        if (name == _scheme)
        {
            options.SessionStore = new FileBasedTicketStore();
        }
    }
}