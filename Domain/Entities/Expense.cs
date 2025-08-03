using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Tutar { get; set; }                        // Toplam tutar
        public decimal OrtakHarcamaTutari { get; set; }           // Kişi başı ortak pay
        public int KaydedenUserId { get; set; }
        public User KaydedenUser { get; set; } = null!;
        public int OdeyenUserId { get; set; }
        public User OdeyenUser { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Opsiyonel: harcama sonrası oluşacak paylaşımlar
        public ICollection<Share> Shares { get; set; } = new List<Share>();
        public ICollection<PersonalExpense> PersonalExpenses { get; set; } = new List<PersonalExpense>();
    }
}
