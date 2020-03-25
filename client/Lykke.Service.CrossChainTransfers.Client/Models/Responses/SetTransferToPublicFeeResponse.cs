﻿using Lykke.Service.CrossChainTransfers.Client.Models.Enums;

namespace Lykke.Service.CrossChainTransfers.Client.Models.Responses
{
    /// <summary>
    /// Response model
    /// </summary>
    public class SetTransferToPublicFeeResponse
    {
        /// <summary>
        /// Error code
        /// </summary>
        public FeesErrorCodes Error { get; set; }
    }
}
