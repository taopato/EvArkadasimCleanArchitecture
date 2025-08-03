// Yol: Domain/Entities/Share.cs
namespace Domain.Entities
{
    public class Share
    {
        public int Id { get; set; }
        public int HarcamaId { get; set; }
        public Expense Harcama { get; set; }
        public int PaylasimUserId { get; set; }
        public User PaylasimUser { get; set; }
        public string PaylasimTuru { get; set; }
        public decimal PaylasimTutar { get; set; }
    }
}
