using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.CrossChainTransfers.Domain.Repositories;
using MAVN.Service.CrossChainTransfers.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MAVN.Service.CrossChainTransfers.MsSqlRepositories.Repositories
{
    public class DeduplicationLogRepository : IDeduplicationLogRepository
    {
        private readonly PostgreSQLContextFactory<CrossChainTransfersContext> _contextFactory;

        public DeduplicationLogRepository(PostgreSQLContextFactory<CrossChainTransfersContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsDuplicateAsync(string key)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var deduplicationEntry = DeduplicationEntity.Create(key, DateTime.UtcNow);

                try
                {
                    await context.DeduplicationLog.AddAsync(deduplicationEntry);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is PostgresException sqlException &&
                        sqlException.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
