using Lykke.Service.CrossChainTransfers.Client.Models.Enums;

namespace Lykke.Service.CrossChainTransfers.Client.Models.Responses
{
    /// <summary>
    /// Response model
    /// </summary>
    public class TransferToExternalResponse
    {
        /// <summary>
        /// Error code
        /// </summary>
        public TransferToExternalErrorCodes Error { get; set; }
    }
}
