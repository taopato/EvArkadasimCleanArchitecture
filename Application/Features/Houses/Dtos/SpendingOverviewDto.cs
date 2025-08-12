using System;
using System.Collections.Generic;

namespace Application.Features.Houses.Queries.GetSpendingOverview
{
    public class SpendingOverviewDto
    {
        public RangeInfo Range { get; set; } = new();
        public Dictionary<string, decimal> UtilitiesTotals { get; set; } = new();
        public List<CategoryTotal> GeneralExpenses { get; set; } = new();
        public List<UserPaymentTotal> PaymentsByUser { get; set; } = new();
        public List<UserOpenBalance> OpenBalances { get; set; } = new();
        public List<RecentBillItem> RecentBills { get; set; } = new();
        public List<RecentExpenseItem> RecentExpenses { get; set; } = new();

        public class RangeInfo { public DateTime From { get; set; } public DateTime To { get; set; } }
        public class CategoryTotal { public string Category { get; set; } = ""; public decimal Total { get; set; } }
        public class UserPaymentTotal { public int UserId { get; set; } public decimal PaymentsTotal { get; set; } }
        public class UserOpenBalance { public int UserId { get; set; } public decimal DebtOpen { get; set; } public decimal CreditOpen { get; set; } }
        public class RecentBillItem { public int BillId { get; set; } public int UtilityType { get; set; } public string Month { get; set; } = ""; public decimal Amount { get; set; } }
        public class RecentExpenseItem { public int ExpenseId { get; set; } public string Category { get; set; } = ""; public decimal Amount { get; set; } public DateTime Date { get; set; } }
    }
}
