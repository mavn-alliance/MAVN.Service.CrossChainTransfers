using JetBrains.Annotations;
using MAVN.Service.CrossChainTransfers.Settings.Settings;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.CrossChainTransfers.Settings
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
