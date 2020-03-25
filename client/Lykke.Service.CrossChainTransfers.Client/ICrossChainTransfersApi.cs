using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.CrossChainTransfers.Client.Models.Requests;
using Lykke.Service.CrossChainTransfers.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Lykke.Service.CrossChainTransfers.Client
{
    // This is an example of service controller interfaces.
    // Actual interface methods must be placed here (not in ICrossChainTransfersClient interface).

    /// <summary>
    /// CrossChainTransfers client API interface.
    /// </summary>
    [PublicAPI]
    public interface ICrossChainTransfersApi
    {
        /// <summary>
        /// Create а request to transfer to external
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/cross-chain-transfers/to-external")]

        Task<TransferToExternalResponse> TransferToExternalAsync([FromBody] TransferToExternalRequest request);
    }
}
