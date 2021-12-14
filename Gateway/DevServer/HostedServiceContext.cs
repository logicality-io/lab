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

        public GatewayLoadBalancerHostedService LoadBalancer
        {
            get => Get<GatewayLoadBalancerHostedService>(nameof(LoadBalancer));
            set => Add(nameof(LoadBalancer), value);
        }

        public Gateway1HostedService Gateway1
        {
            get => Get<Gateway1HostedService>(nameof(Gateway1));
            set => Add(nameof(Gateway1), value);
        }

        public Gateway2HostedService Gateway2
        {
            get => Get<Gateway2HostedService>(nameof(Gateway2));
            set => Add(nameof(Gateway2), value);
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
