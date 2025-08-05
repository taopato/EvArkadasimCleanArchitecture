using Application.Features.Houses.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.Houses.Queries.GetUserDebts
{
    public class GetUserDebtsQuery : IRequest<Response<GetUserDebtsDto>>
    {
        public int UserId { get; set; }
        public int HouseId { get; set; }
    }
}
