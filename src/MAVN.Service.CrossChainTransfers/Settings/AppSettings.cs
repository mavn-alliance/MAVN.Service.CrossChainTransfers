using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.WalletManagement.Client;

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
