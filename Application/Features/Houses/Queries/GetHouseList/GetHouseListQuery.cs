using Application.Features.Houses.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Houses.Queries.GetHouseList
{
    public class GetHouseListQuery : IRequest<List<HouseDto>> { }
}
