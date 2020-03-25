using AutoMapper;
using Lykke.PrivateBlockchain.Definitions;
using Lykke.Service.CrossChainTransfers.Domain.Common;
using Lykke.Service.CrossChainTransfers.Domain.Enums;
using Lykke.Service.CrossChainTransfers.Domain.Models;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace Lykke.Service.CrossChainTransfers.DomainServices.Common
{
    public class BlockchainEventDecoder : IBlockchainEventDecoder
    {
        private readonly EventTopicDecoder _eventTopicDecoder;
        private readonly IMapper _mapper;
        private readonly string _transferedFromPublicNetworkEventSignature;
        private readonly string _transferedToPublicNetworkEventSignature;

        public BlockchainEventDecoder(IMapper mapper)
        {
            _mapper = mapper;
            _eventTopicDecoder = new EventTopicDecoder();
            _transferedFromPublicNetworkEventSignature = $"0x{ABITypedRegistry.GetEvent<TransferredFromPublicNetworkEventDTO>().Sha3Signature}";
            _transferedToPublicNetworkEventSignature = $"0x{ABITypedRegistry.GetEvent<TransferredToPublicNetworkEventDTO>().Sha3Signature}";
        }

        public TransferToExternalEventDto DecodeTransferToExternalEvent(string[] topics, string data)
        {
            var decodedEvent = DecodeEvent<TransferredToPublicNetworkEventDTO>(topics, data);

            return _mapper.Map<TransferToExternalEventDto>(decodedEvent);
        }

        public TransferToInternalEventDto DecodeTransferToInternalEvent(string[] topics, string data)
        {
            var decodedEvent = DecodeEvent<TransferredFromPublicNetworkEventDTO>(topics, data);

            return _mapper.Map<TransferToInternalEventDto>(decodedEvent);
        }

        public BlockchainEventType GetEventType(string topic)
        {
            if (topic == _transferedFromPublicNetworkEventSignature)
                return BlockchainEventType.TransferredFromPublicNetwork;

            if (topic == _transferedToPublicNetworkEventSignature)
                return BlockchainEventType.TransferredToPublicNetwork;

            return BlockchainEventType.Unknown;
        }

        private T DecodeEvent<T>(string[] topics, string data) where T : class, new()
            => _eventTopicDecoder.DecodeTopics<T>(topics, data);
    }
}
