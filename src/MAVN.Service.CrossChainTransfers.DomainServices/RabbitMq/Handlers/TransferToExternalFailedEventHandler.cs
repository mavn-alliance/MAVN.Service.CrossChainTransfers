using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers
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
