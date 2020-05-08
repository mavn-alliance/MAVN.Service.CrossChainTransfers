using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.Enums;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WalletManagement.Client;
using MAVN.Service.WalletManagement.Client.Enums;

namespace MAVN.Service.CrossChainTransfers.DomainServices.Services
{
    public class CrossChainTransfersService : ICrossChainTransfersService
    {
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly IWalletManagementClient _walletManagementClient;
        private readonly ICrossChainWalletLinkerClient _crossChainWalletLinkerClient;
        private readonly IFeesService _feesService;
        private readonly ISettingsService _settingsService;
        private readonly IRabbitPublisher<TransferToExternalRequestedEvent> _transferToExternalRequestedPublisher;

        public CrossChainTransfersService(
            IPrivateBlockchainFacadeClient pbfClient,
            IWalletManagementClient walletManagementClient,
            ICrossChainWalletLinkerClient crossChainWalletLinkerClient,
            IFeesService feesService,
            ISettingsService settingsService,
            IRabbitPublisher<TransferToExternalRequestedEvent> transferToExternalRequestedPublisher)
        {
            _pbfClient = pbfClient;
            _walletManagementClient = walletManagementClient;
            _crossChainWalletLinkerClient = crossChainWalletLinkerClient;
            _feesService = feesService;
            _settingsService = settingsService;
            _transferToExternalRequestedPublisher = transferToExternalRequestedPublisher;
        }
        public async Task<TransferToExternalErrorCodes> TransferToExternalAsync(string customerId, Money18 amount)
        {
            #region Validation

            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            if (amount <= 0)
                return TransferToExternalErrorCodes.InvalidAmount;

            var customerBlockStatus = await _walletManagementClient.Api.GetCustomerWalletBlockStateAsync(customerId);

            if (customerBlockStatus.Error == CustomerWalletBlockStatusError.CustomerNotFound)
                return TransferToExternalErrorCodes.CustomerDoesNotExist;

            if (customerBlockStatus.Status == CustomerWalletActivityStatus.Blocked)
                return TransferToExternalErrorCodes.CustomerWalletBlocked;

            var isCustomerIdValidGuid = Guid.TryParse(customerId, out var customerIdAsGuid);

            if (!isCustomerIdValidGuid)
                return TransferToExternalErrorCodes.CustomerIdIsNotAValidGuid;

            var fee = await _feesService.GetTransfersToPublicFeeAsync();

            var balanceResponse = await _pbfClient.CustomersApi.GetBalanceAsync(customerIdAsGuid);

            if (balanceResponse.Error != CustomerBalanceError.None)
                return TransferToExternalErrorCodes.CustomerWalletMissing;

            var amountAndFeeTotal = amount + fee;

            if (balanceResponse.Total < amountAndFeeTotal)
                return TransferToExternalErrorCodes.NotEnoughBalance;

            var walletLinkerResponse =
                await _crossChainWalletLinkerClient.CustomersApi.GetLinkedPublicAddressAsync(customerIdAsGuid);

            if (walletLinkerResponse.Status != PublicAddressStatus.Linked)
                return TransferToExternalErrorCodes.WalletIsNotLinked;

            #endregion

            await _transferToExternalRequestedPublisher.PublishAsync(new TransferToExternalRequestedEvent
            {
                Amount = amount,
                CustomerId = customerId,
                PrivateBlockchainGatewayContractAddress = _settingsService.GetPrivateBlockchainGatewayContractAddress() ,
                OperationId = Guid.NewGuid().ToString()
            });

            return TransferToExternalErrorCodes.None;
        }
    }
}
