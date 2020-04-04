using System.Numerics;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Falcon.Numerics;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.Common;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.Domain.Services;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers
{
    public class TransferToInternalDetectedEventHandler : ITransferToInternalDetectedEventHandler
    {
        private readonly IBlockchainEncodingService _blockchainEncodingService;
        private readonly ISettingsService _settingsService;
        private readonly IRabbitPublisher<TransferToInternalProcessedEvent> _transferToInternalProcessedPublisher;
        private readonly ILog _log;

        public TransferToInternalDetectedEventHandler(
            IBlockchainEncodingService blockchainEncodingService,
            ISettingsService settingsService,
            IRabbitPublisher<TransferToInternalProcessedEvent> transferToInternalProcessedPublisher,
            ILogFactory logFactory)
        {
            _blockchainEncodingService = blockchainEncodingService;
            _settingsService = settingsService;
            _transferToInternalProcessedPublisher = transferToInternalProcessedPublisher;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string operationId, string privateAddress, string publicAddress, Money18 amount, BigInteger publicTransferId)
        {
            if (string.IsNullOrEmpty(operationId))
            {
                _log.Error(message: "Operation id missing in TransferToInternalDetectedEvent");
                return;
            }

            if (string.IsNullOrEmpty(privateAddress))
            {
                _log.Error(message: "Private address missing in TransferToInternalDetectedEvent");
                return;
            }

            if (string.IsNullOrEmpty(publicAddress))
            {
                _log.Error(message: "Public address missing in TransferToInternalDetectedEvent");
                return;
            }

            if (amount <= 0)
            {
                _log.Error(message: "Invalid amount TransferToInternalDetectedEvent",
                    context: new {privateAddress, amount});
                return;
            }

            var encodedData =
                _blockchainEncodingService.EncodeTransferToInternalData(privateAddress, publicAddress, amount,
                    publicTransferId);
            
            await _transferToInternalProcessedPublisher.PublishAsync(new TransferToInternalProcessedEvent
            {
                PrivateBlockchainGatewayContractAddress = _settingsService.GetPrivateBlockchainGatewayContractAddress(),
                MasterWalletAddress = _settingsService.GetMasterWalletAddress(),
                Data = encodedData,
                OperationId = operationId,
            });
        }
    }
}
