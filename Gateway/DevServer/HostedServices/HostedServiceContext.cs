namespace Logicality.ExampleGateway.DevServer.HostedServices
{
    public class HostedServiceContext
    {
        private readonly Dictionary<string, IHostedService> _hostedServices = new();

        public HostedServiceContext(Action<ILoggingBuilder> configureLogging, bool fixedPorts = true)
        {
            ConfigureLogging = configureLogging;
            FixedPorts = fixedPorts;
        }

        public Action<ILoggingBuilder> ConfigureLogging { get; }

        public bool FixedPorts { get; }

        public Redis Redis
        {
            get => Get<Redis>(nameof(Redis));
            set => Add(nameof(HostedServices.Redis), value);
        }

        public LoadBalancer LoadBalancer
        {
            get => Get<LoadBalancer>(nameof(LoadBalancer));
            set => Add(nameof(LoadBalancer), value);
        }

        public Gateway1 Gateway1
        {
            get => Get<Gateway1>(nameof(Gateway1));
            set => Add(nameof(Gateway1), value);
        }

        public Gateway2 Gateway2
        {
            get => Get<Gateway2>(nameof(Gateway2));
            set => Add(nameof(Gateway2), value);
        }
        public Seq Seq
        {
            get => Get<Seq>(nameof(Seq));
            set => Add(nameof(Seq), value);
        }

        public SignIn SignIn
        {
            get => Get<SignIn>(nameof(SignIn));
            set => Add(nameof(SignIn), value);
        }

        public IdentityProvider IdentityProvider
        {
            get => Get<IdentityProvider>(nameof(IdentityProvider));
            set => Add(nameof(IdentityProvider), value);
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
