using MediatR;

namespace Application.Features.Bills.Commands.FinalizeBill
{
    public class FinalizeBillCommand : IRequest<bool>
    {
        public int BillId { get; set; }
        public int RequestUserId { get; set; } // güvenlik: sadece sorumlu finalize edebilir
    }
}
