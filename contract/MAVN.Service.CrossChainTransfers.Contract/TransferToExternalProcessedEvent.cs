using System.Numerics;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Contract
{
    public class TransferToExternalProcessedEvent
    {
        public string OperationId { get; set; }
        /// <summary>
        /// Id of the Customer
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Amount of tokens to transfer, without the fee
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Transaction hash of the operation
        /// </summary>
        public string TxHash { get; set; }

        /// <summary>
        /// Private wallet address of the customer
        /// </summary>
        public string PrivateAddress { get; set; }

        /// <summary>
        /// Public linked address of the customer
        /// </summary>
        public string PublicAddress { get; set; }

        /// <summary>
        /// Transfer id of the transaction
        /// </summary>
        public BigInteger InternalTransferId { get; set; }
    }
}
