namespace Application.Features.Expenses.Dtos
{
    public class UpdateExpenseDto
    {
        public string Tur { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public int HouseId { get; set; }
        public int OdeyenUserId { get; set; }
        public List<PersonalExpenseDto> SahsiHarcamalar { get; set; } = new();
        public decimal OrtakHarcamaTutari { get; set; }
    }
}