﻿using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Service.CrossChainTransfers.Domain.Enums;

namespace Lykke.Service.CrossChainTransfers.Domain.Services
{
    public interface ICrossChainTransfersService
    {
        Task<TransferToExternalErrorCodes> TransferToExternalAsync(string customerId, Money18 amount);
    }
}
