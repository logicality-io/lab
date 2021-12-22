using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Logicality.ExampleGateway.AuthCookieHandling;

public class RedisCacheTicketStore : ITicketStore
{
    private const    string            KeyPrefix = "AuthSessionStore-";
    private readonly IDistributedCache _cache;

    public RedisCacheTicketStore(RedisCacheOptions options)
    {
        _cache = new RedisCache(options);
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = KeyPrefix + Guid.NewGuid();
        await RenewAsync(key, ticket);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var options    = new DistributedCacheEntryOptions();
        var expiresUtc = ticket.Properties.ExpiresUtc;
        if (expiresUtc.HasValue)
        {
            options.SetAbsoluteExpiration(expiresUtc.Value);
        }
        var serializedTicket = TicketSerializer.Default.Serialize(ticket);
        await _cache.SetAsync(key, serializedTicket, options);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var bytes  = await _cache.GetAsync(key);
        var ticket = TicketSerializer.Default.Deserialize(bytes);
        return ticket;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}