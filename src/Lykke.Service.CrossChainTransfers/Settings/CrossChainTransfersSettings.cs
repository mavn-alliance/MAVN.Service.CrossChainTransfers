using JetBrains.Annotations;
using Lykke.Service.CrossChainTransfers.Settings.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.CrossChainTransfers.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CrossChainTransfersSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public NotificationsSettings Notifications { get; set; }

        public string PrivateBlockchainGatewayContractAddress { get; set; }

        public string MasterWalletAddress { get; set; }
    }
}
