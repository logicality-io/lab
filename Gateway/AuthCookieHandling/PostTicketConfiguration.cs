using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Logicality.ExampleGateway.AuthCookieHandling;

public class PostTicketConfiguration : IPostConfigureOptions<CookieAuthenticationOptions>
{
    private readonly ITicketStore _ticketStore;

    public PostTicketConfiguration(ITicketStore ticketStore)
    {
        _ticketStore = ticketStore;
    }

    public void PostConfigure(string name, CookieAuthenticationOptions options)
    {
        options.SessionStore = _ticketStore;
    }
}