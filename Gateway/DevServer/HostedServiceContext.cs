using Microsoft.Extensions.Hosting;

namespace DevServer
{
    public class HostedServiceContext
    {
        private readonly Dictionary<string, IHostedService> _hostedServices = new();

        public RedisHostedService Redis
        {
            get => Get<RedisHostedService>(nameof(Redis));
            set => Add(nameof(RedisHostedService), value);
        }

        private void Add(string name, IHostedService hostedService)
        {
            lock (_hostedServices)
            {
                _hostedServices.Add(name, hostedService);
            }
        }

        private T Get<T>(string name) where T : IHostedService
        {
            lock (_hostedServices)
            {
                if (!_hostedServices.TryGetValue(name, out var value))
                {
                    throw new InvalidOperationException($"HostedService {name} was not found. Check the hosted services registration sequence.");
                }

                return (T)value;
            }
        }
    }
}
