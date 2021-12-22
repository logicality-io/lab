using Microsoft.AspNetCore.DataProtection;

namespace Logicality.ExampleGateway.AuthCookieHandling;

public class InsecureDataProtectionProvider : IDataProtectionProvider
{
    public IDataProtector CreateProtector(string purpose)
    {
        return new PassThroughDataProtector();
    }
}