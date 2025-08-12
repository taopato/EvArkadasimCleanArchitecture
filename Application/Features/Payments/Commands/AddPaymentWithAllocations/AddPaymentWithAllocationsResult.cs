﻿using System.Collections.Generic;

namespace Application.Features.Payments.Commands.AddPaymentWithAllocations
{
    public class AddPaymentWithAllocationsResult
    {
        public int PaymentId { get; set; }
        public List<AppliedItem> AppliedAllocations { get; set; } = new();

        public class AppliedItem
        {
            public int LedgerEntryId { get; set; }
            public decimal Applied { get; set; }
            public decimal RemainingOnEntry { get; set; }
            public bool Closed { get; set; }
        }
    }
}
