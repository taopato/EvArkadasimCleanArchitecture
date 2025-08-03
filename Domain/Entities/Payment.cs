// Yol: Domain/Entities/Payment.cs
namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BorcluUserId { get; set; }
        public int AlacakliUserId { get; set; }
        public string Not { get; set; }
        public decimal Tutar { get; set; }
        public bool AlacakliOnayi { get; set; }

        public User BorcluUser { get; set; }
        public User AlacakliUser { get; set; }
    }
}
