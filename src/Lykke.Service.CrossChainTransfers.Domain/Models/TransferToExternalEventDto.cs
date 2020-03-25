using System.Numerics;
using Falcon.Numerics;

namespace Lykke.Service.CrossChainTransfers.Domain.Models
{
    public class TransferToExternalEventDto
    {
        public string InternalAddress { get; set; }

        public string PublicAddress { get; set; }

        public Money18 Amount { get; set; }

        public  BigInteger InternalTransferId { get; set; }
    }
}
