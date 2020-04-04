using System;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.Common;
using MAVN.Service.CrossChainTransfers.Domain.Enums;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.Domain.Repositories;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using Lykke.Service.PrivateBlockchainFacade.Client;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers
{
    public class UndecodedEventHandler : IUndecodedEventHandler
    {
        private readonly IBlockchainEventDecoder _blockchainEventDecoder;
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly ISettingsService _settingsService;
        private readonly IRabbitPublisher<TransferToExternalProcessedEvent> _transferToExternalProcessedPublisher;
        private readonly IRabbitPublisher<TransferToInternalCompletedEvent> _transferToInternalCompletedPublisher;
        private readonly IPushNotificationsPublisher _pushNotificationsPublisher;
        private readonly IDeduplicationLogRepository _deduplicationLogRepository;
        private readonly ILog _log;

        public UndecodedEventHandler(
            IBlockchainEventDecoder blockchainEventDecoder,
            IPrivateBlockchainFacadeClient pbfClient,
            ISettingsService settingsService,
            IRabbitPublisher<TransferToExternalProcessedEvent> transferToExternalProcessedPublisher,
            IRabbitPublisher<TransferToInternalCompletedEvent> transferToInternalCompletedPublisher,
            IPushNotificationsPublisher pushNotificationsPublisher,
            IDeduplicationLogRepository deduplicationLogRepository,
            ILogFactory logFactory)
        {
            _blockchainEventDecoder = blockchainEventDecoder;
            _pbfClient = pbfClient;
            _settingsService = settingsService;
            _transferToExternalProcessedPublisher = transferToExternalProcessedPublisher;
            _transferToInternalCompletedPublisher = transferToInternalCompletedPublisher;
            _pushNotificationsPublisher = pushNotificationsPublisher;
            _deduplicationLogRepository = deduplicationLogRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string[] topics, string data, string contractAddress, string operationId, string txHash)
        {
            //This means that the event is raised by another smart contract and we are not interested in it
            if (!string.Equals(contractAddress, _settingsService.GetPrivateBlockchainGatewayContractAddress()
                , StringComparison.InvariantCultureIgnoreCase))
            {
                _log.Info("The contract address differs from the expected one. Event handling will be stopped.",
                    new { expected = _settingsService.GetPrivateBlockchainGatewayContractAddress(), current = contractAddress });

                return;
            }

            var isDuplicate = await _deduplicationLogRepository.IsDuplicateAsync(operationId);

            if (isDuplicate)
            {
                _log.Warning(message: "Duplicated event found, processing won't continue further", context: operationId);
                return;
            }

            var eventType = _blockchainEventDecoder.GetEventType(topics[0]);

            switch (eventType)
            {
                case BlockchainEventType.Unknown:
                    return;
                case BlockchainEventType.TransferredFromPublicNetwork:
                    await HandleTransferToInternalAsync(topics, data, operationId);
                    break;
                case BlockchainEventType.TransferredToPublicNetwork:
                    await HandleTransferToExternalAsync(topics, data, operationId, txHash);
                    break;
                default: throw new InvalidOperationException("Unsupported blockchain event type");
            }
        }

        private async Task HandleTransferToExternalAsync(string[] topics, string data, string operationId, string txHash)
        {
            var transferToExternalData = _blockchainEventDecoder.DecodeTransferToExternalEvent(topics, data);

            var customerResponse =
                await _pbfClient.CustomersApi.GetCustomerIdByWalletAddress(transferToExternalData.InternalAddress);

            if (customerResponse.Error != CustomerWalletAddressError.None)
            {
                _log.Warning("No customer with that internal wallet address during decoding of TransferToExternal",
                    context: new {customerResponse.Error, transferToExternalData.InternalAddress});
            }

            await _transferToExternalProcessedPublisher.PublishAsync(new TransferToExternalProcessedEvent
            {
                OperationId = operationId,
                CustomerId = customerResponse.CustomerId,
                Amount = transferToExternalData.Amount,
                PublicAddress = transferToExternalData.PublicAddress,
                PrivateAddress = transferToExternalData.InternalAddress,
                InternalTransferId = transferToExternalData.InternalTransferId,
                TxHash = txHash
            });
        }

        private async Task HandleTransferToInternalAsync(string[] topics, string data, string operationId)
        {
            var transferToInternalData = _blockchainEventDecoder.DecodeTransferToInternalEvent(topics, data);

            var customerResponse =
                await _pbfClient.CustomersApi.GetCustomerIdByWalletAddress(transferToInternalData.InternalAddress);

            if (customerResponse.Error != CustomerWalletAddressError.None)
            {
                _log.Error(
                    message: "No customer with that internal wallet address during decoding of TransferToInternal",
                    context: new { customerResponse.Error, transferToInternalData.InternalAddress });
                return;
            }

            await _transferToInternalCompletedPublisher.PublishAsync(new TransferToInternalCompletedEvent
            {
                CustomerId = customerResponse.CustomerId,
                Amount = transferToInternalData.Amount,
                PrivateAddress = transferToInternalData.InternalAddress,
                PublicAddress = transferToInternalData.PublicAddress,
                OperationId = operationId
            });

            await _pushNotificationsPublisher.PublishTransferToInternalSucceededAsync(customerResponse.CustomerId,
                transferToInternalData.Amount);
        }

    }
}
