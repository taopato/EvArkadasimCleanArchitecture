using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfLedgerEntryRepository : ILedgerEntryRepository
    {
        private readonly AppDbContext _ctx;
        public EfLedgerEntryRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task AddRangeAsync(IEnumerable<LedgerEntry> entries)
        {
            await _ctx.LedgerEntries.AddRangeAsync(entries);
            await _ctx.SaveChangesAsync();
        }

        public Task<List<LedgerEntry>> GetOpenDebtsAsync(int houseId, int userId)
            => _ctx.LedgerEntries
                   .Where(x => x.HouseId == houseId && x.FromUserId == userId && !x.IsClosed)
                   .AsNoTracking().ToListAsync();

        public Task<List<LedgerEntry>> GetOpenCreditsAsync(int houseId, int userId)
            => _ctx.LedgerEntries
                   .Where(x => x.HouseId == houseId && x.ToUserId == userId && !x.IsClosed)
                   .AsNoTracking().ToListAsync();

        public Task<LedgerEntry?> GetByIdAsync(int id)
            => _ctx.LedgerEntries.FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateAsync(LedgerEntry entry)
        {
            _ctx.LedgerEntries.Update(entry);
            await _ctx.SaveChangesAsync();
        }
    }
}
