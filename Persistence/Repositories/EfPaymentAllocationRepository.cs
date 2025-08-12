using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfPaymentAllocationRepository : IPaymentAllocationRepository
    {
        private readonly AppDbContext _ctx;
        public EfPaymentAllocationRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task AddRangeAsync(IEnumerable<PaymentAllocation> allocations)
        {
            await _ctx.PaymentAllocations.AddRangeAsync(allocations);
            await _ctx.SaveChangesAsync();
        }

        public Task<decimal> GetPaidAmountForLedgerEntryAsync(int ledgerEntryId)
            => _ctx.PaymentAllocations
                   .Where(x => x.LedgerEntryId == ledgerEntryId)
                   .SumAsync(x => (decimal?)x.Amount ?? 0m);
    }
}
