using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AuthCore;

public class FileBasedTicketStore : ITicketStore
{
    private readonly string _directory;

    public FileBasedTicketStore()
    {
        _directory = Path.Combine(Path.GetTempPath(), "tickets");
        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var name             = ticket.Principal.Claims.FirstOrDefault(x => x.Type == "name");
        var filePath         = Path.Combine(_directory, $"{name.Value}.txt");
        var serializedTicket = TicketSerializer.Default.Serialize(ticket);
        await File.WriteAllBytesAsync(filePath, serializedTicket);
        return name.Value;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var filePath         = Path.Combine(_directory, $"{key}.txt");
        var serializedTicket = TicketSerializer.Default.Serialize(ticket);
        await File.WriteAllBytesAsync(filePath, serializedTicket);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var filePath = Path.Combine(_directory, $"{key}.txt");
        if (!File.Exists(filePath)) return null;
        var ticketBytes = await File.ReadAllBytesAsync(filePath);
        return TicketSerializer.Default.Deserialize(ticketBytes);
    }

    public Task RemoveAsync(string key)
    {
        var filePath = Path.Combine(_directory, $"{key}.txt");
        File.Delete(filePath);
        return Task.CompletedTask;
    }

}