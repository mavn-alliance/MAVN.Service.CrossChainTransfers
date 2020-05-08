using System.Numerics;
using MAVN.Numerics;

namespace MAVN.Service.CrossChainTransfers.Domain.Models
{
    public class TransferToExternalEventDto
    {
        public string InternalAddress { get; set; }

        public string PublicAddress { get; set; }

        public Money18 Amount { get; set; }

        public  BigInteger InternalTransferId { get; set; }
    }
}
