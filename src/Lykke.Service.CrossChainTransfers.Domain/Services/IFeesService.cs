using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.CrossChainTransfers.Domain.Enums;

namespace Lykke.Service.CrossChainTransfers.Domain.Services
{
    public interface IFeesService
    {
        Task<FeesError> SetTransfersToPublicFeeAsync(Money18 fee);

        Task<Money18> GetTransfersToPublicFeeAsync();
    }
}
