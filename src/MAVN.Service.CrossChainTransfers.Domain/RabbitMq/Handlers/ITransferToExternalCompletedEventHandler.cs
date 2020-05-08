using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers
{
    public interface ITransferToExternalCompletedEventHandler
    {
        Task HandleAsync(string privateAddress, Money18 amount, string eventId);
    }
}
