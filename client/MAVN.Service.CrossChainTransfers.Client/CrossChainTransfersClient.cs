using Lykke.HttpClientGenerator;

namespace MAVN.Service.CrossChainTransfers.Client
{
    /// <summary>
    /// CrossChainTransfers API aggregating interface.
    /// </summary>
    public class CrossChainTransfersClient : ICrossChainTransfersClient
    {
        /// <summary>Interface to CrossChainTransfers Api.</summary>
        public ICrossChainTransfersApi Api { get; private set; }

        /// <summary>Interface to Fees Api.</summary>
        public IFeesApi FeesApi { get; set; }

        /// <summary>C-tor</summary>
        public CrossChainTransfersClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<ICrossChainTransfersApi>();
            FeesApi = httpClientGenerator.Generate<IFeesApi>();
        }
    }
}
