using Microsoft.AspNetCore.DataProtection;

namespace AuthCore;

public class InsecureDataProtectionProvider : IDataProtectionProvider
{
    public IDataProtector CreateProtector(string purpose)
    {
        return new PassThroughDataProtector();
    }
}