using System.Threading.Tasks;
using MAVN.Job.EthereumBridge.Contract;
using MAVN.Job.QuorumTransactionWatcher.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Sdk;
using MAVN.Service.PrivateBlockchainFacade.Contract.Events;

namespace MAVN.Service.CrossChainTransfers.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly JsonRabbitSubscriber<TransferToExternalCompletedEvent> _transferToExternalCompletedSub;
        private readonly JsonRabbitSubscriber<TransferToInternalDetectedEvent> _transferToInternalDetectedSub;
        private readonly JsonRabbitSubscriber<UndecodedEvent> _undecodedSub;
        private readonly JsonRabbitSubscriber<TransferToExternalFailedEvent> _transferToExternalFailedSub;

        public StartupManager(
            JsonRabbitSubscriber<TransferToExternalCompletedEvent> transferToExternalCompletedSub,
            JsonRabbitSubscriber<TransferToInternalDetectedEvent> transferToInternalDetectedSub,
            JsonRabbitSubscriber<UndecodedEvent> undecodedSub,
            JsonRabbitSubscriber<TransferToExternalFailedEvent> transferToExternalFailedSub)
        {
            _transferToExternalCompletedSub = transferToExternalCompletedSub;
            _transferToInternalDetectedSub = transferToInternalDetectedSub;
            _undecodedSub = undecodedSub;
            _transferToExternalFailedSub = transferToExternalFailedSub;
        }
        public Task StartAsync()
        {
            _transferToExternalCompletedSub.Start();
            _transferToInternalDetectedSub.Start();
            _undecodedSub.Start();
            _transferToExternalFailedSub.Start();

            return Task.CompletedTask;
        }
    }
}
