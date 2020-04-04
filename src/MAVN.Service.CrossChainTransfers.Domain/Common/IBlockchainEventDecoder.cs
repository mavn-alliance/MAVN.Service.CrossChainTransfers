using MAVN.Service.CrossChainTransfers.Domain.Enums;
using MAVN.Service.CrossChainTransfers.Domain.Models;

namespace MAVN.Service.CrossChainTransfers.Domain.Common
{
    public interface IBlockchainEventDecoder
    {
        TransferToExternalEventDto DecodeTransferToExternalEvent(string[] topics, string data);

        TransferToInternalEventDto DecodeTransferToInternalEvent(string[] topics, string data);

        BlockchainEventType GetEventType(string topic);
    }
}
