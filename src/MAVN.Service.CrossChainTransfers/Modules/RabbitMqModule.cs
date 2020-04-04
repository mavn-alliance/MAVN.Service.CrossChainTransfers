using Autofac;
using Common;
using Lykke.Job.EthereumBridge.Contract;
using Lykke.Job.QuorumTransactionWatcher.Contract;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Subscribers;
using MAVN.Service.CrossChainTransfers.Settings;
using Lykke.Service.PrivateBlockchainFacade.Contract.Events;
using Lykke.SettingsReader;

namespace MAVN.Service.CrossChainTransfers.Modules
{
    public class RabbitMqModule : Module
    {
        private const string TransferToExternalRequestedExchangeName = "lykke.wallet.transfertoexternalrequested";
        private const string TransferToExternalProcessedExchangeName = "lykke.wallet.transfertoexternalprocessed";
        private const string TransferToExternalCompletedExchangeName = "lykke.wallet.transfertoexternalcompleted";
        private const string TransferToInternalDetectedExchangeName = "lykke.wallet.transfertointernaldetected";
        private const string TransferToInternalProcessedExchangeName = "lykke.wallet.transfertointernalprocessed";
        private const string TransferToInternalCompletedExchangeName = "lykke.wallet.transfertointernalcompleted";
        private const string TransferToExternalFailedExchangeName = "lykke.wallet.transfertoexternalfailed";
        private const string NotificationSystemPushNotificationsExchangeName = "notificationsystem.command.pushnotification";

        private const string DefaultQueueName = "crosschaintransfers";

        private readonly IReloadingManager<AppSettings> _appSettings;

        public RabbitMqModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var rabbitMqConnString =
                _appSettings.CurrentValue.CrossChainTransfersService.RabbitMq.RabbitMqConnectionString;
            var notificationRabbitMqConnString =
                _appSettings.CurrentValue.CrossChainTransfersService.RabbitMq.NotificationRabbitMqConnectionString;

            builder.RegisterType<JsonRabbitPublisher<TransferToExternalRequestedEvent>>()
                .As<IRabbitPublisher<TransferToExternalRequestedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToExternalRequestedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<JsonRabbitPublisher<TransferToExternalProcessedEvent>>()
                .As<IRabbitPublisher<TransferToExternalProcessedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToExternalProcessedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<JsonRabbitPublisher<TransferToInternalProcessedEvent>>()
                .As<IRabbitPublisher<TransferToInternalProcessedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToInternalProcessedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<JsonRabbitPublisher<TransferToInternalCompletedEvent>>()
                .As<IRabbitPublisher<TransferToInternalCompletedEvent>>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToInternalCompletedExchangeName)
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<UndecodedSubscriber>()
                .As<JsonRabbitSubscriber<UndecodedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", Context.GetEndpointName<UndecodedEvent>())
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();

            builder.RegisterType<TransferToExternalCompletedSubscriber>()
                .As<JsonRabbitSubscriber<TransferToExternalCompletedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToExternalCompletedExchangeName)
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();

            builder.RegisterType<TransferToInternalDetectedSubscriber>()
                .As<JsonRabbitSubscriber<TransferToInternalDetectedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToInternalDetectedExchangeName)
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();

            builder.RegisterType<TransferToExternalFailedSubscriber>()
                .As<JsonRabbitSubscriber<TransferToExternalFailedEvent>>()
                .As<IStopable>()
                .WithParameter("connectionString", rabbitMqConnString)
                .WithParameter("exchangeName", TransferToExternalFailedExchangeName)
                .WithParameter("queueName", DefaultQueueName)
                .SingleInstance();

            builder.RegisterType<PushNotificationsPublisher>()
                .WithParameter("connectionString", notificationRabbitMqConnString)
                .WithParameter("exchangeName", NotificationSystemPushNotificationsExchangeName)
                .As<IPushNotificationsPublisher>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}
