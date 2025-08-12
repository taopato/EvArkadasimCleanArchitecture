using System;

namespace Domain.Entities
{
    // Tek bir Payment ile birden fazla LedgerEntry kısmi/tam kapatma
    public class PaymentAllocation
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }     // mevcut Payments tablosuna FK
        public int LedgerEntryId { get; set; } // kapatılan borç kalemi
        public decimal Amount { get; set; }    // bu Allocation ile kapatılan tutar
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
