namespace Lykke.Service.CrossChainTransfers.Domain.Services
{
    public interface ISettingsService
    {
        string GetMasterWalletAddress();

        string GetPrivateBlockchainGatewayContractAddress();
    }
}
