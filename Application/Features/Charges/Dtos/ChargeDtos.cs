using System;
using System.Collections.Generic;

namespace Application.Features.Charges.Dtos
{
    // GET /api/Charges?houseId=&period=
    public class ChargesListQueryDto
    {
        public int HouseId { get; set; }
        public string Period { get; set; } = ""; // "YYYY-MM"
    }

    // POST /api/Charges/{cycleId}/SetBill  (Variable gider)
    public class SetBillRequest
    {
        public DateTime BillDate { get; set; }
        public string BillNumber { get; set; } = "";
        public string BillDocumentUrl { get; set; } = "";
        public decimal TotalAmount { get; set; }
    }

    // POST /api/Charges/{cycleId}/MarkPaid
    public class MarkPaidRequest
    {
        public DateTime PaidDate { get; set; }
        public string ExternalReceiptUrl { get; set; } = "";
    }

    // GET çıktısı için özet
    public class ChargeCycleSummaryDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Type { get; set; } = "";
        public string AmountMode { get; set; } = "";
        public string Period { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public List<UserShareDto> PerUserShares { get; set; } = new();
        public string Status { get; set; } = "";
        public DateTime? BillDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal FundedAmount { get; set; }
    }

    public class UserShareDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
