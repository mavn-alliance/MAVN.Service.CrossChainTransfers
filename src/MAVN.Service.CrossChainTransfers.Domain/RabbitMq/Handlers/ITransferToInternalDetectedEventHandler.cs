using System.Numerics;
using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers
{
    public interface ITransferToInternalDetectedEventHandler
    {
        Task HandleAsync(string operationId, string privateAddress, string publicAddress, Money18 amount, BigInteger publicTransferId);
    }
}
