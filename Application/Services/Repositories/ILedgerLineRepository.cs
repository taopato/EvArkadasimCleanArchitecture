using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface ILedgerLineRepository
    {
        // Create
        Task<LedgerLine> AddAsync(LedgerLine entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<LedgerLine> entities, CancellationToken ct = default);

        // Save
        Task SaveChangesAsync(CancellationToken ct = default);

        // Read
        Task<IList<LedgerLine>> GetByExpenseIdAsync(int expenseId, CancellationToken ct = default);
        Task<IList<LedgerLine>> GetListAsync(Expression<Func<LedgerLine, bool>> predicate, CancellationToken ct = default);

        // Update (soft delete)
        Task<int> SoftDeleteByExpenseIdAsync(int expenseId, CancellationToken ct = default);
    }
}
