using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface ILedgerEntryRepository
    {
        Task AddRangeAsync(IEnumerable<LedgerEntry> entries);
        Task<List<LedgerEntry>> GetOpenDebtsAsync(int houseId, int userId);
        Task<List<LedgerEntry>> GetOpenCreditsAsync(int houseId, int userId);
        Task<LedgerEntry?> GetByIdAsync(int id);
        Task UpdateAsync(LedgerEntry entry);
    }
}
