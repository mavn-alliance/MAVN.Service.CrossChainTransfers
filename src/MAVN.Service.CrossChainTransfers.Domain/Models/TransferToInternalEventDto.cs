using System.Numerics;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Domain.Models
{
    public class TransferToInternalEventDto
    {
        public string PublicAddress { get; set; }

        public string InternalAddress { get; set; }

        public BigInteger PublicTransferId { get; set; }

        public Money18 Amount { get; set; }
    }
}
