using MediatR;

namespace Application.Features.Users.Queries.GetUserPaymentHistory
{
    public class GetUserPaymentHistoryQuery : IRequest<UserPaymentHistoryDto>
    {
        public int UserId { get; set; }
        public int? HouseId { get; set; }
        public int Limit { get; set; } = 10;
    }
}
