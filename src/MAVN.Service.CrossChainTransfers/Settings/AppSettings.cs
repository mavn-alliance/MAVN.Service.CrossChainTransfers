using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WalletManagement.Client;

namespace MAVN.Service.CrossChainTransfers.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public CrossChainTransfersSettings CrossChainTransfersService { get; set; }

        public PrivateBlockchainFacadeServiceClientSettings PrivateBlockchainFacadeService { get; set; }

        public WalletManagementServiceClientSettings WalletManagementService { get; set; }

        public CrossChainWalletLinkerServiceClientSettings CrossChainWalletLinkerService { get; set; }
    }
}
