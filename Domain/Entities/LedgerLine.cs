using System;

namespace Domain.Entities
{
    public class LedgerLine
    {
        public long Id { get; set; }

        public int HouseId { get; set; }
        public int ExpenseId { get; set; }

        // Borçlu → Alacaklı
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public decimal Amount { get; set; } // 2 ondalık

        // Borcun hesaplamaya girdiği gün (UTC)
        public DateTime PostDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
