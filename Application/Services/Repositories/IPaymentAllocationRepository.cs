using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface IPaymentAllocationRepository
    {
        Task AddRangeAsync(IEnumerable<PaymentAllocation> allocations);
        Task<decimal> GetPaidAmountForLedgerEntryAsync(int ledgerEntryId);
    }
}
