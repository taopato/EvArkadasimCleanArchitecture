namespace Application.Features.RecurringCharges.Dtos
{
    public class CreateRecurringChargeRequest
    {
        public int HouseId { get; set; }
        public string Type { get; set; } = "";          // "Rent" | "Electricity" | ...
        public int PayerUserId { get; set; }
        public string AmountMode { get; set; } = "";     // "Fixed" | "Variable"
        public string SplitPolicy { get; set; } = "";    // "Equal" | "Weight"

        public decimal FixedAmount { get; set; }         // Fixed ise > 0
        public int? DueDay { get; set; }                 // Fixed ise 1-28

        public int PaymentWindowDays { get; set; }       // Variable ise > 0 (non-nullable)

        public Dictionary<int, decimal>? Weights { get; set; }  // SplitPolicy=Weight
        public string StartMonth { get; set; } = "2025-09";
        // IsActive alanınız yoksa handler default true atıyor.
    }

    public class UpdateRecurringChargeRequest
    {
        public int PayerUserId { get; set; }                   // >0 ise günceller
        public string SplitPolicy { get; set; } = "";          // "Equal" | "Weight"
        public Dictionary<int, decimal>? Weights { get; set; } // Weight için

        public decimal? FixedAmount { get; set; }
        public int? DueDay { get; set; }
        public int? PaymentWindowDays { get; set; }
        // IsActive varsa ekleyebilirsiniz; yoksa handler dokunmaz
        public bool IsActive { get; set; } = true;
    }

    public class CancelRecurringChargeRequest
    {
        public string? EffectiveFromPeriod { get; set; } // "YYYY-MM" (opsiyonel, ilerisi için)
    }

    public class RecurringChargeDto
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public string Type { get; set; } = "";
        public int PayerUserId { get; set; }
        public string AmountMode { get; set; } = "";
        public string SplitPolicy { get; set; } = "";
        public decimal FixedAmount { get; set; }
        public int? DueDay { get; set; }
        public int PaymentWindowDays { get; set; }
        public string StartMonth { get; set; } = "";
        public bool IsActive { get; set; }
    }
}
