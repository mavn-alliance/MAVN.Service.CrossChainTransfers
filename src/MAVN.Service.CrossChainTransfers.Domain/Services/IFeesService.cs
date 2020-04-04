using System.Threading.Tasks;
using Falcon.Numerics;
using MAVN.Service.CrossChainTransfers.Domain.Enums;

namespace MAVN.Service.CrossChainTransfers.Domain.Services
{
    public interface IFeesService
    {
        Task<FeesError> SetTransfersToPublicFeeAsync(Money18 fee);

        Task<Money18> GetTransfersToPublicFeeAsync();
    }
}
