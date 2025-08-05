using Application.Features.Houses.Dtos;
using MediatR;

namespace Application.Features.Houses.Queries.GetHouseDetail
{
    public class GetHouseDetailQuery : IRequest<HouseDetailDto>
    {
        public int Id { get; set; }
    }
}
