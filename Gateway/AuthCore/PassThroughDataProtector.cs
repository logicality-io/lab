using Microsoft.AspNetCore.DataProtection;

namespace AuthCore;

public class PassThroughDataProtector : IDataProtector
{
    public byte[] Protect(byte[] plaintext)
    {
        return plaintext;
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        return protectedData;
    }

    public IDataProtector CreateProtector(string purpose)
    {
        return new PassThroughDataProtector();
    }
}