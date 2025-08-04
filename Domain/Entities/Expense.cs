using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;  // “Tur” olarak map ediyoruz
        public decimal Tutar { get; set; }
        public decimal OrtakHarcamaTutari { get; set; }

        public int HouseId { get; set; }
        public House House { get; set; } = null!;

        public int OdeyenUserId { get; set; }
        public User OdeyenUser { get; set; } = null!;

        public int KaydedenUserId { get; set; }
        public User KaydedenUser { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<PersonalExpense> PersonalExpenses { get; set; } = new List<PersonalExpense>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();
    }
}
