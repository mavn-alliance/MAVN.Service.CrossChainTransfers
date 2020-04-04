using System.Threading.Tasks;

namespace MAVN.Service.CrossChainTransfers.Domain.Repositories
{
    public interface IDeduplicationLogRepository
    {
        Task<bool> IsDuplicateAsync(string key);
    }
}
