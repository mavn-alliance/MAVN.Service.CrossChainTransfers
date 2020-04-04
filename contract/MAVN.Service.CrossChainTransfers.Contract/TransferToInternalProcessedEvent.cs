
namespace MAVN.Service.CrossChainTransfers.Contract
{
    public class TransferToInternalProcessedEvent
    {
        /// <summary>
        /// Master wallet address
        /// </summary>
        public string MasterWalletAddress { get; set; }

        /// <summary>
        /// Address of the smart contract
        /// </summary>
        public string PrivateBlockchainGatewayContractAddress { get; set; }

        /// <summary>
        /// Encoded data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public string OperationId { get; set; }
    }
}
