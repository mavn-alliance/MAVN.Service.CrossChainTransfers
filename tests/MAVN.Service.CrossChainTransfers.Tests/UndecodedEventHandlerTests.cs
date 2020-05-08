using System;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainTransfers.Domain.Common;
using MAVN.Service.CrossChainTransfers.Domain.Enums;
using MAVN.Service.CrossChainTransfers.Domain.Models;
using MAVN.Service.CrossChainTransfers.Domain.RabbitMq.Publishers;
using MAVN.Service.CrossChainTransfers.Domain.Repositories;
using MAVN.Service.CrossChainTransfers.Domain.Services;
using MAVN.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.PrivateBlockchainFacade.Client.Models;
using Moq;
using Xunit;

namespace MAVN.Service.CrossChainTransfers.Tests
{
    public class UndecodedEventHandlerTests
    {
        private const string FakeContractAddress = "address";
        private const string FakeOperationId = "operationId";
        private const string FakeInternalAddress = "internal-address";
        private const string FakeData = "data";
        private const string FakeCustomerId = "custId";
        private const string FakeTxHash = "0xHash";

        private readonly string[] _fakeTopics = { "topic1", "topic2" };
        private readonly Mock<IBlockchainEventDecoder> _eventDecoderMock = new Mock<IBlockchainEventDecoder>();
        private readonly Mock<IPrivateBlockchainFacadeClient> _pbfClientMock = new Mock<IPrivateBlockchainFacadeClient>();
        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();
        private readonly Mock<IRabbitPublisher<TransferToExternalProcessedEvent>>
            _transferToExternalProcessedPublisherMock = new Mock<IRabbitPublisher<TransferToExternalProcessedEvent>>();
        private readonly Mock<IRabbitPublisher<TransferToInternalCompletedEvent>>
            _transferToInternalCompletedPublisherMock = new Mock<IRabbitPublisher<TransferToInternalCompletedEvent>>();
        private readonly Mock<IPushNotificationsPublisher> _pushNotificationsPublisherMock = new Mock<IPushNotificationsPublisher>();
        private readonly Mock<IDeduplicationLogRepository> _deduplicationLogMock = new Mock<IDeduplicationLogRepository>();

        [Fact]
        public async Task HandleAsync_ContractAddressIsNotPrivateBlockchainGatewayOne_EventDecoderNotCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, "AddressOfNoInterest", FakeOperationId, FakeTxHash);

            _eventDecoderMock.Verify(x => x.GetEventType(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_EventIsWithUnknownForUsType_StatusUpdaterNotCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            _eventDecoderMock.Setup(x => x.GetEventType(_fakeTopics[0]))
                .Returns(BlockchainEventType.Unknown);

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _eventDecoderMock.Verify(x => x.DecodeTransferToExternalEvent(It.IsAny<string[]>(), It.IsAny<string>()), Times.Never);
            _eventDecoderMock.Verify(x => x.DecodeTransferToInternalEvent(It.IsAny<string[]>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_EventIsTransferToExternal_CustomerWalletMissing_PublisherCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            _eventDecoderMock.Setup(x => x.GetEventType(_fakeTopics[0]))
                .Returns(BlockchainEventType.TransferredToPublicNetwork);

            _eventDecoderMock.Setup(x => x.DecodeTransferToExternalEvent(_fakeTopics, FakeData))
                .Returns(new TransferToExternalEventDto{InternalAddress = FakeInternalAddress});

            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakeInternalAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    Error = CustomerWalletAddressError.CustomerWalletMissing
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _transferToExternalProcessedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<TransferToExternalProcessedEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_EventIsTransferToExternal_PublisherCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            _eventDecoderMock.Setup(x => x.GetEventType(_fakeTopics[0]))
                .Returns(BlockchainEventType.TransferredToPublicNetwork);

            _eventDecoderMock.Setup(x => x.DecodeTransferToExternalEvent(_fakeTopics, FakeData))
                .Returns(new TransferToExternalEventDto { InternalAddress = FakeInternalAddress });

            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakeInternalAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    CustomerId = FakeCustomerId
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _transferToExternalProcessedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<TransferToExternalProcessedEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_EventIsTransferToInternal_CustomerWalletMissing_PublisherNotCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            _eventDecoderMock.Setup(x => x.GetEventType(_fakeTopics[0]))
                .Returns(BlockchainEventType.TransferredFromPublicNetwork);

            _eventDecoderMock.Setup(x => x.DecodeTransferToInternalEvent(_fakeTopics, FakeData))
                .Returns(new TransferToInternalEventDto { InternalAddress = FakeInternalAddress });

            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakeInternalAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    Error = CustomerWalletAddressError.CustomerWalletMissing
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _transferToInternalCompletedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<TransferToInternalCompletedEvent>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_EventIsTransferToInternal_PublisherCalled()
        {
            _settingsServiceMock.Setup(x => x.GetPrivateBlockchainGatewayContractAddress())
                .Returns(FakeContractAddress);

            _eventDecoderMock.Setup(x => x.GetEventType(_fakeTopics[0]))
                .Returns(BlockchainEventType.TransferredFromPublicNetwork);

            _eventDecoderMock.Setup(x => x.DecodeTransferToInternalEvent(_fakeTopics, FakeData))
                .Returns(new TransferToInternalEventDto { InternalAddress = FakeInternalAddress });

            _pbfClientMock.Setup(x => x.CustomersApi.GetCustomerIdByWalletAddress(FakeInternalAddress))
                .ReturnsAsync(new CustomerIdByWalletAddressResponse
                {
                    CustomerId = FakeCustomerId
                });

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _transferToInternalCompletedPublisherMock.Verify(x => x.PublishAsync(It.IsAny<TransferToInternalCompletedEvent>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_EventIsDuplicated_EventDecoderNotCalled()
        {
            _deduplicationLogMock.Setup(x => x.IsDuplicateAsync(FakeOperationId))
                .ReturnsAsync(true);

            var sut = CreateSutInstance();

            await sut.HandleAsync(_fakeTopics, FakeData, FakeContractAddress, FakeOperationId, FakeTxHash);

            _eventDecoderMock.Verify(x => x.GetEventType(It.IsAny<string>()), Times.Never);
        }

        private UndecodedEventHandler CreateSutInstance()
        {
            return new UndecodedEventHandler(
                _eventDecoderMock.Object,
                _pbfClientMock.Object,
                _settingsServiceMock.Object,
                _transferToExternalProcessedPublisherMock.Object,
                _transferToInternalCompletedPublisherMock.Object,
                _pushNotificationsPublisherMock.Object,
                _deduplicationLogMock.Object,
                EmptyLogFactory.Instance);
        }
    }
}
