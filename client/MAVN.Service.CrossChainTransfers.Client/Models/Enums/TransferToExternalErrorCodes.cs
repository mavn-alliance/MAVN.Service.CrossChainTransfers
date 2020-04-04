namespace MAVN.Service.CrossChainTransfers.Client.Models.Enums
{
    /// <summary>
    /// Error codes
    /// </summary>
    public enum TransferToExternalErrorCodes
    {
        /// <summary>
        /// No error
        /// </summary>
        None,
        /// <summary>
        /// Provided amount is not valid
        /// </summary>
        InvalidAmount,
        /// <summary>
        /// Customer does not exist in the system
        /// </summary>
        CustomerDoesNotExist,
        /// <summary>
        /// Customer's wallet is blocked so he is not able to transfer tokens
        /// </summary>
        CustomerWalletBlocked,
        /// <summary>
        /// Provided customer id is not a valid guid
        /// </summary>
        CustomerIdIsNotAValidGuid,
        /// <summary>
        /// Customer does not have a wallet in the system
        /// </summary>
        CustomerWalletMissing,
        /// <summary>
        /// Customer does not have enough balance
        /// </summary>
        NotEnoughBalance,
        /// <summary>
        /// Customer's internal wallet is not linked to a public one
        /// </summary>
        WalletIsNotLinked,

    }
}
