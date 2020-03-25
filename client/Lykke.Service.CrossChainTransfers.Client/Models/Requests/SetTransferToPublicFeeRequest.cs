using System.ComponentModel.DataAnnotations;
using Falcon.Numerics;

namespace Lykke.Service.CrossChainTransfers.Client.Models.Requests
{
    /// <summary>
    /// Request model
    /// </summary>
    public class SetTransferToPublicFeeRequest
    {
        /// <summary>
        /// The fee amount
        /// </summary>
        [Required]
        public Money18 Fee { get; set; }
    }
}
