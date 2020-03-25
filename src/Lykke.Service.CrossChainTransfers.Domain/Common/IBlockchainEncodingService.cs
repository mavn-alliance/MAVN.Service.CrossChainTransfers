﻿using System.Numerics;
using Falcon.Numerics;

namespace Lykke.Service.CrossChainTransfers.Domain.Common
{
    public interface IBlockchainEncodingService
    {
        string EncodeTransferToInternalData(string privateAddress, string publicAddress, Money18 amount, BigInteger publicTransferId);
    }
}
