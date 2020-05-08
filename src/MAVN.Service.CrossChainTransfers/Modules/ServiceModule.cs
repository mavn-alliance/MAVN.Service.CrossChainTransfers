using Autofac;
using Lykke.Sdk;
using MAVN.Service.CrossChainTransfers.Domain.Common;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using MAVN.Service.CrossChainTransfers.DomainServices.Common;
using MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers;
using MAVN.Service.CrossChainTransfers.DomainServices.Services;
using MAVN.Service.CrossChainTransfers.Services;
using MAVN.Service.CrossChainTransfers.Settings;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WalletManagement.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.CrossChainTransfers.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterWalletManagementClient(_appSettings.CurrentValue.WalletManagementService, null);
            builder.RegisterPrivateBlockchainFacadeClient(_appSettings.CurrentValue.PrivateBlockchainFacadeService, null);
            builder.RegisterCrossChainWalletLinkerClient(_appSettings.CurrentValue.CrossChainWalletLinkerService, null);

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();

            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter("privateBlockchainGatewayContractAddress",
                    _appSettings.CurrentValue.CrossChainTransfersService.PrivateBlockchainGatewayContractAddress)
                .WithParameter("masterWalletAddress",
                    _appSettings.CurrentValue.CrossChainTransfersService.MasterWalletAddress)
                .SingleInstance();

            builder.RegisterType<UndecodedEventHandler>()
                .As<IUndecodedEventHandler>()
                .SingleInstance();

            builder.RegisterType<TransferToInternalDetectedEventHandler>()
                .As<ITransferToInternalDetectedEventHandler>()
                .SingleInstance();

            builder.RegisterType<TransferToExternalFailedEventHandler>()
                .As<ITransferToExternalFailedEventHandler>()
                .SingleInstance();

            builder.RegisterType<TransferToExternalCompletedEventHandler>()
                .As<ITransferToExternalCompletedEventHandler>()
                .SingleInstance();

            builder.RegisterType<CrossChainTransfersService>()
                .As<ICrossChainTransfersService>()
                .SingleInstance();

            builder.RegisterType<BlockchainEncodingService>()
                .As<IBlockchainEncodingService>()
                .SingleInstance();

            builder.RegisterType<BlockchainEventDecoder>()
                .As<IBlockchainEventDecoder>()
                .SingleInstance();

            builder.RegisterType<PushNotificationsSettingsService>()
                .WithParameter("crossChainWithdrawSuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainTransfersService.Notifications.PushNotifications
                        .CrossChainWithdrawSuccessfulTemplateId)
                .WithParameter("crossChainWithdrawUnsuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainTransfersService.Notifications.PushNotifications
                        .CrossChainWithdrawUnsuccessfulTemplateId)
                .WithParameter("crossChainDepositSuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainTransfersService.Notifications.PushNotifications
                        .CrossChainDepositSuccessfulTemplateId)
                .WithParameter("crossChainDepositUnsuccessfulTemplateId",
                    _appSettings.CurrentValue.CrossChainTransfersService.Notifications.PushNotifications
                        .CrossChainDepositUnsuccessfulTemplateId)
                .As<IPushNotificationsSettingsService>()
                .SingleInstance();

            builder.RegisterType<FeesService>()
                .As<IFeesService>()
                .SingleInstance();
        }
    }
}
