using System.Numerics;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.CrossChainTransfers.Contract;
using Lykke.Service.CrossChainTransfers.Domain.Common;
using Lykke.Service.CrossChainTransfers.Domain.Services;
using Lykke.Service.CrossChainTransfers.DomainServices.RabbitMq.Handlers;
using Moq;
using Xunit;

namespace Lykke.Service.CrossChainTransfers.Tests
{
    public class TransferToInternalDetectedEventHandlerTests
    {
        private const string FakePrivateAddress = "0xPrivate";
        private const string FakeOperationId = "OpId";
        private const string FakePublicAddress = "0xPublic";
        private const long FakePublicTransferId = 1234;
        private const long FakeAmount = 100;

        private readonly Mock<IBlockchainEncodingService> _blockchainEncodingServiceMock = new Mock<IBlockchainEncodingService>();
        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();
        private readonly Mock<IRabbitPublisher<TransferToInternalProcessedEvent>> _transferToInternalProcessedPublisher
            = new Mock<IRabbitPublisher<TransferToInternalProcessedEvent>>();

        [Theory]
        [InlineData(null, FakePrivateAddress, FakePublicAddress, FakeAmount)]
        [InlineData(FakeOperationId, null, FakePublicAddress, FakeAmount)]
        [InlineData(FakeOperationId, FakePrivateAddress, null, FakeAmount)]
        [InlineData(FakeOperationId, FakePrivateAddress, FakePublicAddress, 0)]
        [InlineData(FakeOperationId, FakePrivateAddress, FakePublicAddress, -10)]
        public async Task HandleAsync_InvalidInputParameters_PublisherNotCalled
            (string operationId, string privateAddress, string publicAddress, long amount)
        {
            var sut = CreateSutInstance();

            await sut.HandleAsync(operationId, privateAddress, publicAddress, amount, FakePublicTransferId);

            _transferToInternalProcessedPublisher.Verify(
                x => x.PublishAsync(It.IsAny<TransferToInternalProcessedEvent>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_EverythingValid_PublisherCalled()
        {
            var sut = CreateSutInstance();

            await sut.HandleAsync(FakeOperationId, FakePrivateAddress, FakePublicAddress, FakeAmount, FakePublicTransferId);

            _transferToInternalProcessedPublisher.Verify(
                x => x.PublishAsync(It.IsAny<TransferToInternalProcessedEvent>()), Times.Once);
        }

        private TransferToInternalDetectedEventHandler CreateSutInstance()
        {
            return new TransferToInternalDetectedEventHandler(
                _blockchainEncodingServiceMock.Object,
                _settingsServiceMock.Object,
                _transferToInternalProcessedPublisher.Object,
                EmptyLogFactory.Instance);
        }
    }
}
