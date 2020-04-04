using MAVN.Service.CrossChainTransfers.Client.Models.Enums;

namespace MAVN.Service.CrossChainTransfers.Client.Models.Responses
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
