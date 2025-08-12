using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfUtilityBillRepository : IUtilityBillRepository
    {
        private readonly AppDbContext _ctx;
        public EfUtilityBillRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<Bill> AddAsync(Bill bill)
        {
            var e = await _ctx.Bills.AddAsync(bill);
            await _ctx.SaveChangesAsync();
            return e.Entity;
        }

        public Task<Bill?> GetAsync(int id)
            => _ctx.Bills.Include(x => x.Shares)
                         .Include(x => x.Documents)
                         .FirstOrDefaultAsync(x => x.Id == id);

        public Task<Bill?> GetByHouseMonthAsync(int houseId, UtilityType type, string month)
            => _ctx.Bills.Include(x => x.Shares)
                         .Include(x => x.Documents)
                         .FirstOrDefaultAsync(x => x.HouseId == houseId && x.UtilityType == type && x.Month == month);

        public Task<List<Bill>> GetRecentAsync(int houseId, UtilityType? type, int limit)
        {
            var q = _ctx.Bills.Where(x => x.HouseId == houseId);
            if (type.HasValue) q = q.Where(x => x.UtilityType == type.Value);
            return q.OrderByDescending(x => x.CreatedAt).Take(limit).AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(Bill bill)
        {
            _ctx.Bills.Update(bill);
            await _ctx.SaveChangesAsync();
        }
    }
}
