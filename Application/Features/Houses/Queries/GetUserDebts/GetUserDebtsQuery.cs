// Application/Features/Houses/Queries/GetUserDebts/GetUserDebtsQuery.cs
using Application.Features.Houses.Dtos;
using Core.Utilities.Results;
using MediatR;

namespace Application.Features.Houses.Queries.GetUserDebts
{
    public class GetUserDebtsQuery : IRequest<Response<GetUserDebtsDto>>
    {
        public int UserId { get; }
        public int HouseId { get; }

        public GetUserDebtsQuery(int userId, int houseId)
        {
            UserId = userId;
            HouseId = houseId;
        }
    }
}
