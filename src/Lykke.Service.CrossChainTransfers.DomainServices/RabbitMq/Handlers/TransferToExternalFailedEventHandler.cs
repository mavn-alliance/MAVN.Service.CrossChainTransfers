using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using Lykke.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;

namespace Lykke.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers
{
    public class TransferToExternalFailedEventHandler : ITransferToExternalFailedEventHandler
    {
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;

        public TransferToExternalFailedEventHandler(IPushNotificationsPublisher pushNotificationsPublisher)
        {
            _pushNotificationsPublisher = pushNotificationsPublisher;
        }

        public Task HandleAsync(string customerId, Money18 amount)
            => _pushNotificationsPublisher.PublishTransferToExternalFailedAsync(customerId, amount);
        
    }
}
