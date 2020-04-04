using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.CrossChainTransfers.Client.Models.Requests;
using MAVN.Service.CrossChainTransfers.Client.Models.Responses;
using Refit;

namespace MAVN.Service.CrossChainTransfers.Client
{
    /// <summary>
    /// FeesApi
    /// </summary>
    [PublicAPI]
    public interface IFeesApi
    {
        /// <summary>
        /// Get transfers to public fee
        /// </summary>
        /// <returns></returns>
        [Get("/api/fees/transfer-to-public")]
        Task<TransferToPublicFeeResponse> GetTransferToPublicFeeAsync();

        /// <summary>
        /// Set transfers to public fee
        /// </summary>
        /// <param name="request"></param>
        [Post("/api/fees/transfer-to-public")]

        Task<SetTransferToPublicFeeResponse> SetTransferToPublicFeeAsync(SetTransferToPublicFeeRequest request);
    }
}
