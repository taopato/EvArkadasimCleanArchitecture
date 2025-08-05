namespace Application.Features.Payments.Dtos
{
    public class CreatedPaymentResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = "Ödeme başarıyla kaydedildi.";
    }
}