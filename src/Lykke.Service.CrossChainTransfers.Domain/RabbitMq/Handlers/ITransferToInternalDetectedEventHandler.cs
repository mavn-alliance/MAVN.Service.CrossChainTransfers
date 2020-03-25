using System.Numerics;
using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Service.CrossChainTransfers.Domain.RabbitMq.Handlers
{
    public interface ITransferToInternalDetectedEventHandler
    {
        Task HandleAsync(string operationId, string privateAddress, string publicAddress, Money18 amount, BigInteger publicTransferId);
    }
}
