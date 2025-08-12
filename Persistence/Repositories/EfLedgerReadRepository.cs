using System.Linq;
using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfLedgerReadRepository : ILedgerReadRepository
    {
        private readonly AppDbContext _ctx;
        public EfLedgerReadRepository(AppDbContext ctx) => _ctx = ctx;

        public IQueryable<LedgerEntry> Query() => _ctx.LedgerEntries.AsQueryable();
    }
}
