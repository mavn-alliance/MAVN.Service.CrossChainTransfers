using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Service.CrossChainTransfers.Domain.RabbitMq.Publishers
{
    public interface IPushNotificationsPublisher
    {
        Task PublishTransferToExternalSucceededAsync(string customerId, Money18 amount);

        Task PublishTransferToExternalFailedAsync(string customerId, Money18 amount);

        Task PublishTransferToInternalSucceededAsync(string customerId, Money18 amount);

        Task PublishTransferToInternalFailedAsync(string customerId, Money18 amount);
    }
}
