using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public partial class Expense
    {
        public int Id { get; set; }

        // Asıl kolon
        public string Description { get; set; } = string.Empty; // “Tur” ile eşleyeceğiz
        public decimal Tutar { get; set; }
        public decimal OrtakHarcamaTutari { get; set; }
        public bool IsActive { get; set; } = true;

        public int HouseId { get; set; }
        public House House { get; set; } = null!;

        public int OdeyenUserId { get; set; }
        public User OdeyenUser { get; set; } = null!;

        public int KaydedenUserId { get; set; }
        public User KaydedenUser { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<PersonalExpense> PersonalExpenses { get; set; } = new List<PersonalExpense>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();

        // --- Alias'lar (DB'ye yazılmaz) ---
        [NotMapped]
        public string Tur
        {
            get => Description;
            set => Description = value ?? string.Empty;
        }
        [NotMapped]
        public DateTime KayitTarihi
        {
            get => CreatedDate;
            set => CreatedDate = value;
        }
    }
}
