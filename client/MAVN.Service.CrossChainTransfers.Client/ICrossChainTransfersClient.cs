using JetBrains.Annotations;

namespace MAVN.Service.CrossChainTransfers.Client
{
    /// <summary>
    /// CrossChainTransfers client interface.
    /// </summary>
    [PublicAPI]
    public interface ICrossChainTransfersClient
    {
        /// <summary>Application Api interface</summary>
        ICrossChainTransfersApi Api { get; }

        /// <summary>Interface to Fees Api.</summary>
        IFeesApi FeesApi { get; }
    }
}
