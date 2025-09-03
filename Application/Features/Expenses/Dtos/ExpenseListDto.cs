namespace Application.Features.Expenses.Dtos
{
    public class ExpenseListDto
    {
        public int Id { get; set; }
        public string Tur { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int HouseId { get; set; }
        public int OdeyenUserId { get; set; }
        public int KaydedenUserId { get; set; }
        public DateTime KayitTarihi { get; set; }
        public string OdeyenKullaniciAdi { get; set; } = string.Empty;
        public string KaydedenKullaniciAdi { get; set; } = string.Empty;

    }
}