using System;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.Domain.Repositories;
using Lykke.Service.PrivateBlockchainFacade.Client;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers
{
    public class TransferToExternalCompletedEventHandler : ITransferToExternalCompletedEventHandler
    {
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;
        private readonly IDeduplicationLogRepository _deduplicationLogRepository;
        private readonly ILog _log;

        public TransferToExternalCompletedEventHandler(
            IPrivateBlockchainFacadeClient pbfClient,
            IPushNotificationsPublisher pushNotificationsPublisher,
            IDeduplicationLogRepository deduplicationLogRepository,
            ILogFactory logFactory)
        {
            _pbfClient = pbfClient;
            _pushNotificationsPublisher = pushNotificationsPublisher;
            _deduplicationLogRepository = deduplicationLogRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string privateAddress, Money18 amount, string eventId)
        {
            var isDuplicate = await _deduplicationLogRepository.IsDuplicateAsync(eventId);

            if (isDuplicate)
            {
                _log.Warning(message: "Duplicated TransferToExternalCompletedEvent found, processing won't continue further", context: eventId);
                return;
            }

            if (string.IsNullOrEmpty(privateAddress))
            {
                _log.Error(message:"Empty private address in TransferToExternalCompleted");
                return;
            }

            var customerIdResponse = await _pbfClient.CustomersApi.GetCustomerIdByWalletAddress(privateAddress);

            if (customerIdResponse.Error != CustomerWalletAddressError.None)
            {
                _log.Error(message:"Cannot find CustomerId from wallet address to handle TransferToExternalCompletedEvent",
                    context: new {privateAddress, customerIdResponse.Error});
                return;
            }

            await _pushNotificationsPublisher.PublishTransferToExternalSucceededAsync(customerIdResponse.CustomerId, amount);
        }
    }
}
