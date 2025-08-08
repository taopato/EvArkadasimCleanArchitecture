// Application/Features/Houses/Dtos/GetUserDebtsDto.cs
using System.Collections.Generic;

namespace Application.Features.Houses.Dtos
{
    public class GetUserDebtsDto
    {
        public int UserId { get; set; }
        public int HouseId { get; set; }

        public decimal ToplamBorc { get; set; }
        public decimal ToplamAlacak { get; set; }
        public decimal NetDurum => ToplamAlacak - ToplamBorc;

        // Harcama detayları (her expense için)
        public List<UserDebtDetailDto> Detaylar { get; set; } = new();

        // Kullanıcı bazlı net durumlar (kime ne kadar borç/alacak)
        public List<KullaniciBazliDurumDto> KullaniciBazliDurumlar { get; set; } = new();
    }

    public class UserDebtDetailDto
    {
        public int ExpenseId { get; set; }
        public string Tur { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int OdeyenUserId { get; set; }
        public string OdeyenKullaniciAdi { get; set; } = string.Empty;
        public decimal PaylasimTutar { get; set; }
    }

    public class KullaniciBazliDurumDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
