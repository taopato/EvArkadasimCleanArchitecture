// Yol: Domain/Entities/Expense.cs
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Tur { get; set; }
        public decimal Tutar { get; set; }
        public int HouseId { get; set; }
        public House House { get; set; }
        public int OdeyenUserId { get; set; }
        public User OdeyenUser { get; set; }
        public int KaydedenUserId { get; set; }
        public User KaydedenUser { get; set; }
        public DateTime Tarih { get; set; }
        public decimal OrtakHarcamaTutari { get; set; }

        public List<PersonalExpense> SahsiHarcama { get; set; }
        public ICollection<Share> Shares { get; set; }
    }
}
