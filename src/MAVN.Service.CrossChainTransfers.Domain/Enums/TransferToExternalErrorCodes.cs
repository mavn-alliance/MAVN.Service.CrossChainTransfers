namespace MAVN.Service.CrossChainTransfers.Domain.Enums
{
    public enum TransferToExternalErrorCodes
    {
        None,
        InvalidAmount,
        CustomerDoesNotExist,
        CustomerWalletBlocked,
        CustomerIdIsNotAValidGuid,
        CustomerWalletMissing,
        NotEnoughBalance,
        WalletIsNotLinked,
    }
}
