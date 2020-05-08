using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using Lykke.Service.PrivateBlockchainFacade.Contract.Events;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Subscribers
{
    public class TransferToExternalFailedSubscriber : JsonRabbitSubscriber<TransferToExternalFailedEvent>
    {
        private readonly ITransferToExternalFailedEventHandler _handler;
        private readonly ILog _log;

        public TransferToExternalFailedSubscriber(
            ITransferToExternalFailedEventHandler handler,
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _handler = handler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(TransferToExternalFailedEvent message)
        {
            await _handler.HandleAsync(message.CustomerId, Money18.Parse(message.Amount.ToString()));
            _log.Info("Processed TransferToExternalFailedEvent", message);
        }
    }
}
