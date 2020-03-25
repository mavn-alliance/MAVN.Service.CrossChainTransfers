using System.Threading.Tasks;

namespace Lykke.Service.CrossChainTransfers.Domain.Repositories
{
    public interface IDeduplicationLogRepository
    {
        Task<bool> IsDuplicateAsync(string key);
    }
}
