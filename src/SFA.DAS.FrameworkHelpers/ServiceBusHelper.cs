using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using System.Threading.Tasks;

namespace SFA.DAS.FrameworkHelpers
{
    public class ServiceBusHelper : IAsyncDisposable
    {
        private IEndpointInstance _endpointInstance;
        public bool IsRunning { get; private set; }

        public async Task Start(string connectionString)
        {
            if (IsRunning) return;

            var endpointConfiguration = UseAzureServiceBusSendOnly(connectionString);

            _endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            IsRunning = true;
        }

        public async Task Stop()
        {
            if (!IsRunning) return;
            await _endpointInstance.Stop();
            IsRunning = false;
        }

        public async ValueTask DisposeAsync() => await Stop();

        public async Task Publish(object message) => await _endpointInstance.Publish(message);

        public static EndpointConfiguration UseAzureServiceBusSendOnly(
            string connectionString)
        {
            var config = new EndpointConfiguration("SendOnly")
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            config.SendOnly();

            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(connectionString);
            transport.Transactions(TransportTransactionMode.None);

            return config;
        }
    }
}
