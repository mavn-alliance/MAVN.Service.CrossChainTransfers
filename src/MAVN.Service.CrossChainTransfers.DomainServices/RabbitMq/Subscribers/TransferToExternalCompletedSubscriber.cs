using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Job.EthereumBridge.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Numerics;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Subscribers
{
    public class TransferToExternalCompletedSubscriber : JsonRabbitSubscriber<TransferToExternalCompletedEvent>
    {
        private readonly ITransferToExternalCompletedEventHandler _handler;
        private readonly ILog _log;

        public TransferToExternalCompletedSubscriber(
            ITransferToExternalCompletedEventHandler handler,
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _handler = handler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(TransferToExternalCompletedEvent message)
        {
            await _handler.HandleAsync(message.PrivateAddress, Money18.Parse(message.Amount.ToString()), message.EventId);
            _log.Info("Processed TransferToExternalCompletedEvent", message);
        }
    }
}
