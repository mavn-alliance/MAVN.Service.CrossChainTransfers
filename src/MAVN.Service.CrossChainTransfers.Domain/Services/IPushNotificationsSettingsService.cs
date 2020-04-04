namespace MAVN.Service.CrossChainTransfers.Domain.Services
{
    public interface IPushNotificationsSettingsService
    {
        string CrossChainWithdrawSuccessfulTemplateId { get; }
        string CrossChainWithdrawUnsuccessfulTemplateId { get; }
        string CrossChainDepositSuccessfulTemplateId { get; }
        string CrossChainDepositUnsuccessfulTemplateId { get; }
    }
}
