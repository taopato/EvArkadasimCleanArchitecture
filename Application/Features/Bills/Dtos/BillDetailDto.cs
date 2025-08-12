using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.Features.Bills.Dtos
{
    public class BillDetailDto
    {
        public int BillId { get; set; }
        public int HouseId { get; set; }
        public UtilityType UtilityType { get; set; }
        public string Month { get; set; } = null!;
        public int ResponsibleUserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? FinalizedAt { get; set; }

        public List<BillShareItem> Shares { get; set; } = new();
        public List<BillDocumentItem> Documents { get; set; } = new();

        public class BillShareItem
        {
            public int UserId { get; set; }
            public decimal Amount { get; set; }
        }

        public class BillDocumentItem
        {
            public int Id { get; set; }
            public string FileName { get; set; } = null!;
            public string FileUrl { get; set; } = null!;
            public DateTime UploadedAt { get; set; }
        }
    }
}
