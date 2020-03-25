using System;
using System.Threading.Tasks;

namespace Lykke.Service.CrossChainTransfers.Domain.RabbitMq.Handlers
{
    public interface IUndecodedEventHandler
    {
        Task HandleAsync(string[] topics, string data, string contractAddress, string operationId, string txHash);
    }
}
