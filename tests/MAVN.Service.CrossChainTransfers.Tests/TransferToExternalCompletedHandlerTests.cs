using System.Threading.Tasks;
using Falcon.Numerics;
using Lykke.Logs;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.Domain.Repositories;
using MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.PrivateBlockchainFacade.Client.Models;
using Moq;
using Xunit;

namespace MAVN.Service.CrossChainTransfers.Tests
{
    public class TransferToExternalCompletedHandlerTests
    {
        private Money18 ValidAmount = 10;
        private const string FakePrivateAddress = "0xAddress";
        private const string FakeCustomerId = "custId";
        private const string FakeEventId = "eventId";

        private readonly Mock<IPrivateBlockchainFacadeClient> _pbfClientMock = new Mock<IPrivateBlockchainFacadeClient>();
        private readonly Mock<IPushNotificationsPublisher> _pushNotificationsPublisherMock = new Mock<IPushNotificationsPublisher>();
        private readonly Mock<IDeduplicationLogRepository> _deduplicationLogMock = new Mock<IDeduplicationLogRepository>();

        [Fact]
        public async Task HandleAsync_MissingPrivateAddress_PrivateBlockchainFacadeNotCalled()
        {
            var sut = CreateSutInstance();

            await sut.HandleAsync(null, ValidAmount, FakeEventId);

            _pbfClientMock.Verify(x => x.CustomersApi.GetCustomerIdByWalletAddress(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CustomerDoesNotExistWithThisWalletAddress_PushNotificationsPublisherNotCalled()
        {
            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakePrivateAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    Error = CustomerWalletAddressError.CustomerWalletMissing
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(FakePrivateAddress, ValidAmount, FakeEventId);

            _pushNotificationsPublisherMock.Verify(
                x => x.PublishTransferToExternalSucceededAsync(It.IsAny<string>(), It.IsAny<Money18>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync__PushNotificationsPublisherCalled()
        {
            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakePrivateAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    Error = CustomerWalletAddressError.None,
                    CustomerId = FakeCustomerId
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(FakePrivateAddress, ValidAmount, FakeEventId);

            _pushNotificationsPublisherMock.Verify(
                x => x.PublishTransferToExternalSucceededAsync(FakeCustomerId, ValidAmount), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_DuplicatedEvent_PushNotificationsPublisherNotCalled()
        {
            _deduplicationLogMock.Setup(x => x.IsDuplicateAsync(FakeEventId))
                .ReturnsAsync(true);

            var sut = CreateSutInstance();

            await sut.HandleAsync(FakePrivateAddress, ValidAmount, FakeEventId);

            _pushNotificationsPublisherMock.Verify(
                x => x.PublishTransferToExternalSucceededAsync(FakeCustomerId, ValidAmount), Times.Never);
        }

        private TransferToExternalCompletedEventHandler CreateSutInstance()
        {
            return new TransferToExternalCompletedEventHandler(
                _pbfClientMock.Object,
                _pushNotificationsPublisherMock.Object,
                _deduplicationLogMock.Object,
                EmptyLogFactory.Instance);
        }
    }
}
