using MAVN.Service.CrossChainTransfers.Domain.Services;

namespace MAVN.Service.CrossChainTransfers.DomainServices.Services
{
    public class PushNotificationsSettingsService : IPushNotificationsSettingsService
    {
        public PushNotificationsSettingsService(
            string crossChainWithdrawSuccessfulTemplateId,
            string crossChainWithdrawUnsuccessfulTemplateId,
            string crossChainDepositSuccessfulTemplateId,
            string crossChainDepositUnsuccessfulTemplateId)
        {
            CrossChainWithdrawSuccessfulTemplateId = crossChainWithdrawSuccessfulTemplateId;
            CrossChainWithdrawUnsuccessfulTemplateId = crossChainWithdrawUnsuccessfulTemplateId;
            CrossChainDepositSuccessfulTemplateId = crossChainDepositSuccessfulTemplateId;
            CrossChainDepositUnsuccessfulTemplateId = crossChainDepositUnsuccessfulTemplateId;
        }

        public string CrossChainWithdrawSuccessfulTemplateId { get; }
        public string CrossChainWithdrawUnsuccessfulTemplateId { get; }
        public string CrossChainDepositSuccessfulTemplateId { get; }
        public string CrossChainDepositUnsuccessfulTemplateId { get; }
    }
}
