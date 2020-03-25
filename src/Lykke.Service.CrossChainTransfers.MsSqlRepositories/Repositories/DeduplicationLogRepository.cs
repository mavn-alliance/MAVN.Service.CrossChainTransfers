using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Service.CrossChainTransfers.Domain.Repositories;
using Lykke.Service.CrossChainTransfers.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.CrossChainTransfers.MsSqlRepositories.Repositories
{
    public class DeduplicationLogRepository : IDeduplicationLogRepository
    {
        private readonly MsSqlContextFactory<CrossChainTransfersContext> _contextFactory;

        public DeduplicationLogRepository(MsSqlContextFactory<CrossChainTransfersContext> contextFactory)
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
                    if (e.InnerException is SqlException sqlException &&
                        sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
