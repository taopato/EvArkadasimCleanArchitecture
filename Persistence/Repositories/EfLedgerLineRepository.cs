using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfLedgerLineRepository : ILedgerLineRepository
    {
        private readonly AppDbContext _ctx;
        public EfLedgerLineRepository(AppDbContext context) => _ctx = context;

        public async Task<LedgerLine> AddAsync(LedgerLine entity, CancellationToken ct = default)
        {
            await _ctx.Set<LedgerLine>().AddAsync(entity, ct);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<LedgerLine> entities, CancellationToken ct = default)
        {
            await _ctx.Set<LedgerLine>().AddRangeAsync(entities, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);

        public async Task<IList<LedgerLine>> GetByExpenseIdAsync(int expenseId, CancellationToken ct = default)
        {
            return await _ctx.Set<LedgerLine>()
                .Where(x => x.ExpenseId == expenseId)
                .ToListAsync(ct);
        }

        public async Task<IList<LedgerLine>> GetListAsync(Expression<Func<LedgerLine, bool>> predicate, CancellationToken ct = default)
        {
            return await _ctx.Set<LedgerLine>()
                .Where(predicate)
                .ToListAsync(ct);
        }

        public async Task<int> SoftDeleteByExpenseIdAsync(int expenseId, CancellationToken ct = default)
        {
            // EF Core 7+
            return await _ctx.Set<LedgerLine>()
                .Where(x => x.ExpenseId == expenseId && x.IsActive)
                .ExecuteUpdateAsync(upd => upd
                    .SetProperty(l => l.IsActive, l => false)
                    .SetProperty(l => l.UpdatedAt, l => DateTime.UtcNow),
                    ct);
        }
    }
}
