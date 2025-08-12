using System;
using Domain.Enums;

namespace Application.Features.Payments.Queries.GetPendingPayments
{
    public class PendingPaymentDto
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public int BorcluUserId { get; set; }
        public int AlacakliUserId { get; set; }
        public string BorcluUserName { get; set; } = string.Empty;

        public decimal Tutar { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // string döndürüyoruz
        public DateTime OdemeTarihi { get; set; }
        public string? Aciklama { get; set; }
        public string DekontUrl { get; set; } = string.Empty;
        public string Status { get; set; } = PaymentStatus.Pending.ToString();
    }
}
