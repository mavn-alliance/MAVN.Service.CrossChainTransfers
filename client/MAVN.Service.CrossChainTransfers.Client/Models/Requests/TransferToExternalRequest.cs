using System.ComponentModel.DataAnnotations;
using Falcon.Numerics;

namespace MAVN.Service.CrossChainTransfers.Client.Models.Requests
{
    /// <summary>
    /// Request model
    /// </summary>
    public class TransferToExternalRequest
    {
        /// <summary>
        /// Id of the customer
        /// </summary>
        [Required]
        public string CustomerId { get; set; }

        /// <summary>
        /// Amount of tokens to transfer
        /// </summary>
        public Money18 Amount { get; set; }
    }
}
