using System.Linq;
using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfBillReadRepository : IBillReadRepository
    {
        private readonly AppDbContext _ctx;
        public EfBillReadRepository(AppDbContext ctx) => _ctx = ctx;

        public IQueryable<Bill> Query() => _ctx.Bills.AsQueryable();
    }
}
