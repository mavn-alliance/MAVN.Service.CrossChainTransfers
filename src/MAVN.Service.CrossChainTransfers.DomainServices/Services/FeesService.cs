using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.PrivateBlockchainFacade.Client.Models;
using FeesError = MAVN.Service.CrossChainTransfers.Domain.Enums.FeesError;

namespace MAVN.Service.CrossChainTransfers.DomainServices.Services
{
    public class FeesService : IFeesService
    {
        private readonly IPrivateBlockchainFacadeClient _pbfClient;

        public FeesService(IPrivateBlockchainFacadeClient pbfClient)
        {
            _pbfClient = pbfClient;
        }

        public async Task<FeesError> SetTransfersToPublicFeeAsync(Money18 fee)
        {
            if (fee < 0 || fee > int.MaxValue)
                return FeesError.InvalidFee;

            var result =
                await _pbfClient.FeesApi.SetTransferToPublicFeeAsync(
                    new SetTransferToPublicFeeRequestModel { Fee = fee });

            return (FeesError) result.Error;
        }

        public async Task<Money18> GetTransfersToPublicFeeAsync()
        {
            var result = await _pbfClient.FeesApi.GetTransferToPublicFeeAsync();

            return result.Fee;
        }
    }
}
