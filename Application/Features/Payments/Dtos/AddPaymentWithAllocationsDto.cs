using System.Collections.Generic;

namespace Application.Features.Payments.Commands.AddPaymentWithAllocations
{
    public class AddPaymentWithAllocationsDto
    {
        public int HouseId { get; set; }
        public int PayerUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }

        public List<AllocationItem> Allocations { get; set; } = new();

        public class AllocationItem
        {
            public int LedgerEntryId { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
