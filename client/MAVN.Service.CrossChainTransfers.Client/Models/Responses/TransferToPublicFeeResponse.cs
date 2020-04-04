using Falcon.Numerics;

namespace MAVN.Service.CrossChainTransfers.Client.Models.Responses
{
    /// <summary>
    /// response model
    /// </summary>
    public class TransferToPublicFeeResponse
    {
        /// <summary>
        /// The fee amount
        /// </summary>
        public Money18 Fee { get; set; }
    }
}
