using System;
using Domain.Enums;

namespace Application.Features.Bills.Dtos
{
    public class CreateBillDto
    {
        public int HouseId { get; set; }
        public UtilityType UtilityType { get; set; }
        public string Month { get; set; } = null!; // "YYYY-MM"
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }

        // Geçici: Sorumluyu requestten alıyoruz (JWT entegrasyonu yoksa)
        public int ResponsibleUserId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
