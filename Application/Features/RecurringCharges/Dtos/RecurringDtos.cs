public class CreateRecurringChargeRequest
{
    public int HouseId { get; set; }
    public string Type { get; set; } = "Rent";   // "Rent" | "Internet" | "Electric"...
    public int PayerUserId { get; set; }
    public string AmountMode { get; set; } = "Fixed"; // "Fixed" | "Variable"
    public decimal? FixedAmount { get; set; }
    public string SplitPolicy { get; set; } = "Equal"; // "Equal" | "Weight"
    public Dictionary<int, decimal>? Weights { get; set; }
    public int? DueDay { get; set; }            // Fixed için
    public int PaymentWindowDays { get; set; } = 5; // Variable için
    public string StartMonth { get; set; } = "";     // "YYYY-MM"
}

public class RecurringChargeDto
{
    public int Id { get; set; }
    public int HouseId { get; set; }
    public string Type { get; set; } = "";
    public int PayerUserId { get; set; }
    public string AmountMode { get; set; } = "";
    public decimal? FixedAmount { get; set; }
    public string SplitPolicy { get; set; } = "";
    public int? DueDay { get; set; }
    public int PaymentWindowDays { get; set; }
    public string StartMonth { get; set; } = "";
    public bool IsActive { get; set; }
}
