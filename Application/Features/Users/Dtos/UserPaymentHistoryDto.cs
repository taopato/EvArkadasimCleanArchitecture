using System;
using System.Collections.Generic;

namespace Application.Features.Users.Queries.GetUserPaymentHistory
{
    public class UserPaymentHistoryDto
    {
        public OpenBalancesDto OpenBalances { get; set; } = new();
        public List<HistoryItem> Recent { get; set; } = new();

        public class OpenBalancesDto { public decimal DebtOpen { get; set; } public decimal CreditOpen { get; set; } }

        public class HistoryItem
        {
            public string Type { get; set; } = "";  // "payment" | "ledger"
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public int? ToUserId { get; set; }
            public string? Role { get; set; }       // "debt" | "credit" (ledger için)
            public decimal? Remaining { get; set; } // ledger için
            public bool? Closed { get; set; }
            public string? Note { get; set; }
        }
    }
}
