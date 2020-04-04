using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.CrossChainTransfers.Client 
{
    /// <summary>
    /// CrossChainTransfers client settings.
    /// </summary>
    public class CrossChainTransfersServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
