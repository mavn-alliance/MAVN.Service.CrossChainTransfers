using Lykke.Service.CrossChainTransfers.Domain.Enums;
using Lykke.Service.CrossChainTransfers.Domain.Models;

namespace Lykke.Service.CrossChainTransfers.Domain.Common
{
    public interface IBlockchainEventDecoder
    {
        TransferToExternalEventDto DecodeTransferToExternalEvent(string[] topics, string data);

        TransferToInternalEventDto DecodeTransferToInternalEvent(string[] topics, string data);

        BlockchainEventType GetEventType(string topic);
    }
}
