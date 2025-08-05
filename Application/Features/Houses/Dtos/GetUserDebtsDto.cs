namespace Application.Features.Houses.Dtos
{
    public class GetUserDebtsDto
    {
        public int UserId { get; set; }
        public int HouseId { get; set; }
        public decimal ToplamBorc { get; set; }
        public decimal ToplamAlacak { get; set; }
        public decimal NetDurum => ToplamAlacak - ToplamBorc;
        public List<UserDebtDetailDto> Detaylar { get; set; } = new();
    }

    public class UserDebtDetailDto
    {
        public int ExpenseId { get; set; }
        public string Tur { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int OdeyenUserId { get; set; }
        public string OdeyenKullaniciAdi { get; set; } = string.Empty;
        public decimal PaylasimTutari { get; set; }
    }
}
