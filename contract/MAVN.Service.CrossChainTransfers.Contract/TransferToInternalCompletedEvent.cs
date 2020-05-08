using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Contract
{
    public class TransferToInternalCompletedEvent
    {
        /// <summary>
        /// Unique id of the operation
        /// </summary>
        public string OperationId { get; set; }
        /// <summary>
        /// Id of the customer
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Amount of tokens
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Private wallet address of the customer
        /// </summary>
        public string PrivateAddress { get; set; }

        /// <summary>
        /// Public linked address of the customer
        /// </summary>
        public string PublicAddress { get; set; }
    }
}
