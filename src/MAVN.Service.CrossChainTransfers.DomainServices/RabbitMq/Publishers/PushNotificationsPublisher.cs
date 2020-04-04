using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using Lykke.Common.Log;
using Falcon.Numerics;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using Lykke.Service.NotificationSystem.SubscriberContract;

namespace MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Publishers
{
    public class PushNotificationsPublisher : JsonRabbitPublisher<PushNotificationEvent>, IPushNotificationsPublisher
    {
        private readonly IPushNotificationsSettingsService _pushNotificationsSettingsService;

        public PushNotificationsPublisher(
            IPushNotificationsSettingsService pushNotificationsSettingsService,
            ILogFactory logFactory,
            string connectionString,
            string exchangeName)
            : base(logFactory, connectionString, exchangeName)
        {
            _pushNotificationsSettingsService = pushNotificationsSettingsService;
        }

        public Task PublishTransferToExternalSucceededAsync(string customerId, Money18 amount)
        {
            return PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}",
                MessageTemplateId = _pushNotificationsSettingsService.CrossChainWithdrawSuccessfulTemplateId,
                TemplateParameters = new Dictionary<string, string> { { "Amount", amount.ToString() } },
                CustomPayload = new Dictionary<string, string> { { "route", "wallet" } }
            });
        }

        public Task PublishTransferToExternalFailedAsync(string customerId, Money18 amount)
        {
            return PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}",
                MessageTemplateId = _pushNotificationsSettingsService.CrossChainWithdrawUnsuccessfulTemplateId,
                TemplateParameters = new Dictionary<string, string> { { "Amount", amount.ToString() } },
                CustomPayload = new Dictionary<string, string> { { "route", "wallet" } }
            });
        }

        public Task PublishTransferToInternalSucceededAsync(string customerId, Money18 amount)
        {
            return PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}",
                MessageTemplateId = _pushNotificationsSettingsService.CrossChainDepositSuccessfulTemplateId,
                TemplateParameters = new Dictionary<string, string> { { "Amount", amount.ToString() } },
                CustomPayload = new Dictionary<string, string> { { "route", "wallet" } }
            });
        }

        public Task PublishTransferToInternalFailedAsync(string customerId, Money18 amount)
        {
            return PublishAsync(new PushNotificationEvent
            {
                CustomerId = customerId,
                Source = $"{AppEnvironment.Name} - {AppEnvironment.Version}",
                MessageTemplateId = _pushNotificationsSettingsService.CrossChainDepositUnsuccessfulTemplateId,
                TemplateParameters = new Dictionary<string, string> { { "Amount", amount.ToString() } },
                CustomPayload = new Dictionary<string, string> { { "route", "wallet" } }
            });
        }
    }
}
