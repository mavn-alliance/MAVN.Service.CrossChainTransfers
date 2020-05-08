using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.Enums;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using MAVN.Service.CrossChainTransfers.DomainServices.Services;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.PrivateBlockchainFacade.Client.Models;
using MAVN.Service.WalletManagement.Client;
using MAVN.Service.WalletManagement.Client.Enums;
using MAVN.Service.WalletManagement.Client.Models.Responses;
using Moq;
using Xunit;

namespace MAVN.Service.CrossChainTransfers.Tests
{
    public class CrossChainTransfersServiceTests
    {
        private const string FakeCustomerId = "17706d7e-1e8a-40f8-a0a3-8ecd7076570d";
        private const string InvalidCustomerId = "invalid";
        private Money18 FakeAmount = 1;

        private readonly Mock<IPrivateBlockchainFacadeClient> _pbfClientMock = new Mock<IPrivateBlockchainFacadeClient>();
        private readonly Mock<IWalletManagementClient> _wmClientMock = new Mock<IWalletManagementClient>();
        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();
        private readonly Mock<IFeesService> _feesServiceMock = new Mock<IFeesService>();
        private readonly Mock<ICrossChainWalletLinkerClient> _walletLinkerClientMock = new Mock<ICrossChainWalletLinkerClient>();
        private readonly Mock<IRabbitPublisher<TransferToExternalRequestedEvent>>
            _transferToExternalRequestedPublisherMock = new Mock<IRabbitPublisher<TransferToExternalRequestedEvent>>();
        

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task TransferToExternalAsync_CustomerIdIsNullOrEmpty_ExceptionThrown(string customerId)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                 sut.TransferToExternalAsync(customerId, FakeAmount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task TransferToExternalAsync_InvalidAmount_ErrorCodeReturned(int amount)
        {
            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, amount);

            Assert.Equal(TransferToExternalErrorCodes.InvalidAmount, result);
        }

        [Fact]
        public async Task TransferToExternalAsync_CustomerNotFound_ErrorCodeReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Error = CustomerWalletBlockStatusError.CustomerNotFound });

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.CustomerDoesNotExist, result);
        }

        [Fact]
        public async Task TransferToExternalAsync_CustomerWalletIsBlocked_ErrorCodeReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Blocked });

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.CustomerWalletBlocked, result);
        }

        [Fact]
        public async Task TransferToExternalAsync_CustomerIdIsNotAValidGuid_ErrorCodeReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(InvalidCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(InvalidCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.CustomerIdIsNotAValidGuid, result);
        }

        [Fact]
        public async Task TransferToExternalAsync_CustomerWalletMissing_ErrorCodeReturned()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            _pbfClientMock.Setup(x => x.CustomersApi.GetBalanceAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new CustomerBalanceResponseModel {Error = CustomerBalanceError.CustomerWalletMissing});

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.CustomerWalletMissing, result);
        }

        [Theory]
        [InlineData(100, 0, 99)]
        [InlineData(99, 1, 99)]
        [InlineData(99, 10, 100)]
        public async Task TransferToExternalAsync_NotEnoughBalance_ErrorCodeReturned(long amount, long fee, long totalBalance)
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            _pbfClientMock.Setup(x => x.CustomersApi.GetBalanceAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new CustomerBalanceResponseModel { Total = totalBalance });

            _feesServiceMock.Setup(x => x.GetTransfersToPublicFeeAsync())
                .ReturnsAsync(fee);

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, amount);

            Assert.Equal(TransferToExternalErrorCodes.NotEnoughBalance, result);
        }

        [Theory]
        [InlineData(PublicAddressStatus.PendingConfirmation)]
        [InlineData(PublicAddressStatus.NotLinked)]
        public async Task TransferToExternalAsync_WalletIsNotLinked_ErrorCodeReturned(PublicAddressStatus status)
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            _pbfClientMock.Setup(x => x.CustomersApi.GetBalanceAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new CustomerBalanceResponseModel { Total = (long)FakeAmount });

            _walletLinkerClientMock.Setup(x => x.CustomersApi.GetLinkedPublicAddressAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new PublicAddressResponseModel {Status = status});

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.WalletIsNotLinked, result);
        }

        [Fact]
        public async Task TransferToExternalAsync_EverythingValid_TransferToExternalRequestedPublisherCalled()
        {
            _wmClientMock.Setup(x => x.Api.GetCustomerWalletBlockStateAsync(FakeCustomerId))
                .ReturnsAsync(new CustomerWalletBlockStatusResponse { Status = CustomerWalletActivityStatus.Active });

            _pbfClientMock.Setup(x => x.CustomersApi.GetBalanceAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new CustomerBalanceResponseModel { Total = (long)FakeAmount });

            _walletLinkerClientMock.Setup(x => x.CustomersApi.GetLinkedPublicAddressAsync(Guid.Parse(FakeCustomerId)))
                .ReturnsAsync(new PublicAddressResponseModel { Error = PublicAddressError.None, Status = PublicAddressStatus.Linked});

            var sut = CreateSutInstance();

            var result = await sut.TransferToExternalAsync(FakeCustomerId, FakeAmount);

            Assert.Equal(TransferToExternalErrorCodes.None, result);
        }

        private CrossChainTransfersService CreateSutInstance()
        {
            return new CrossChainTransfersService(
                _pbfClientMock.Object,
                _wmClientMock.Object,
                _walletLinkerClientMock.Object,
                _feesServiceMock.Object,
                _settingsServiceMock.Object,
                _transferToExternalRequestedPublisherMock.Object);
        }
    }
}
