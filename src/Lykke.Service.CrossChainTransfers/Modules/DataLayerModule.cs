using Autofac;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainTransfers.Domain.Repositories;
using Lykke.Service.CrossChainTransfers.MsSqlRepositories;
using Lykke.Service.CrossChainTransfers.MsSqlRepositories.Repositories;
using Lykke.Service.CrossChainTransfers.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.CrossChainTransfers.Modules
{
    public class DataLayerModule : Module
    {
        private readonly DbSettings _dbSettings;

        public DataLayerModule(IReloadingManager<AppSettings> appSettings)
        {
            _dbSettings = appSettings.CurrentValue.CrossChainTransfersService.Db;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _dbSettings.DataConnString,
                connString => new CrossChainTransfersContext(connString, false),
                dbConn => new CrossChainTransfersContext(dbConn));

            builder.RegisterType<DeduplicationLogRepository>()
                .As<IDeduplicationLogRepository>()
                .SingleInstance();
        }
    }
}
