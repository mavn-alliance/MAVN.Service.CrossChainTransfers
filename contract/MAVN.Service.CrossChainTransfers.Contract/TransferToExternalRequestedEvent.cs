using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Contract
{
    /// <summary>
    /// Event which is used to request a transfer to the external ethereum network
    /// </summary>
    public class TransferToExternalRequestedEvent
    {
        /// <summary>
        /// Id of the Customer
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Amount of tokens to transfer
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Address of the Private Blockchain Gateway smart contract
        /// </summary>
        public string PrivateBlockchainGatewayContractAddress { get; set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public string OperationId { get; set; }
    }
}
