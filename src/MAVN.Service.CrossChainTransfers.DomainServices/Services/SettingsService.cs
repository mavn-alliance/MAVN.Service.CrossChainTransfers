using MAVN.Service.CrossChainTransfers.Domain.Services;

namespace MAVN.Service.CrossChainTransfers.DomainServices.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _privateBlockchainGatewayContractAddress;
        private readonly string _masterWalletAddress;

        public SettingsService(
            string privateBlockchainGatewayContractAddress,
            string masterWalletAddress)
        {
            _privateBlockchainGatewayContractAddress = privateBlockchainGatewayContractAddress;
            _masterWalletAddress = masterWalletAddress;
        }

        public string GetMasterWalletAddress()
            => _masterWalletAddress;

        public string GetPrivateBlockchainGatewayContractAddress()
            => _privateBlockchainGatewayContractAddress;
    }
}
