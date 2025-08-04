using System;

namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int HouseId { get; set; }       // eklendi
        public House House { get; set; } = null!;

        public int BorcluUserId { get; set; }
        public User BorcluUser { get; set; } = null!;

        public int AlacakliUserId { get; set; }
        public User AlacakliUser { get; set; } = null!;

        public decimal Tutar { get; set; }
        public DateTime CreatedDate { get; set; }       // eklendi
    }
}
