using System.Numerics;
using Falcon.Numerics;
using MAVN.PrivateBlockchain.Definitions;
using MAVN.Service.CrossChainTransfers.Domain.Common;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace MAVN.Service.CrossChainTransfers.DomainServices.Common
{
    public class BlockchainEncodingService : IBlockchainEncodingService
    {
        private readonly FunctionCallEncoder _functionCallEncoder;

        public BlockchainEncodingService()
        {
            _functionCallEncoder = new FunctionCallEncoder();
        }

        public string EncodeTransferToInternalData(string privateAddress, string publicAddress, Money18 amount, BigInteger publicTransferId)
        {
            var func = new TransferFromPublicNetworkFunction
            {
                Amount = amount.ToAtto(),
                InternalAccount = privateAddress,
                PublicAccount = publicAddress,
                PublicTransferId = publicTransferId,
            };

            return EncodeRequestData(func);
        }

        private string EncodeRequestData<T>(T func)
            where T : class, new()
        {
            var abiFunc = ABITypedRegistry.GetFunctionABI<T>();
            var result = _functionCallEncoder.EncodeRequest(func, abiFunc.Sha3Signature);

            return result;
        }
    }
}
