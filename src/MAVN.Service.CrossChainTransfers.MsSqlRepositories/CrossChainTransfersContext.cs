using System.Data.Common;
using JetBrains.Annotations;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainTransfers.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.CrossChainTransfers.MsSqlRepositories
{
    public class CrossChainTransfersContext : PostgreSQLContext
    {
        private const string Schema = "cross_chain_transfers";

        internal DbSet<DeduplicationEntity> DeduplicationLog { get; set; }

        [UsedImplicitly]
        public CrossChainTransfersContext() : base(Schema)
        {
        }

        public CrossChainTransfersContext(string connectionString, bool isTraceEnabled) : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public CrossChainTransfersContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }

        public CrossChainTransfersContext(DbConnection dbConnection) : base(Schema, dbConnection)
        {
        }

        protected override void OnMAVNModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
