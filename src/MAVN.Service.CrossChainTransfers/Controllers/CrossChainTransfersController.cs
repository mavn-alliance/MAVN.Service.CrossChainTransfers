using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainTransfers.Client.Models.Enums;
using MAVN.Service.CrossChainTransfers.Client.Models.Requests;
using MAVN.Service.CrossChainTransfers.Client.Models.Responses;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CrossChainTransfers.Controllers
{
    [Route("api/cross-chain-transfers")]
    [ApiController]
    public class CrossChainTransfersController : ControllerBase, ICrossChainTransfersApi
    {
        private readonly ICrossChainTransfersService _crossChainTransfersService;

        public CrossChainTransfersController(ICrossChainTransfersService crossChainTransfersService)
        {
            _crossChainTransfersService = crossChainTransfersService;
        }

        /// <summary>
        /// Create а request to transfer to external
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("to-external")]
        [ProducesResponseType(typeof(TransferToExternalResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<TransferToExternalResponse> TransferToExternalAsync([FromBody] TransferToExternalRequest request)
        {
            var result = await _crossChainTransfersService.TransferToExternalAsync(request.CustomerId, request.Amount);

            return new TransferToExternalResponse { Error = (TransferToExternalErrorCodes)result };
        }
    }
}
