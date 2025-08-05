using Application.Features.Houses.Dtos;
using Core.Utilities.Results;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Houses.Queries.GetHouseMembers
{
    public class GetHouseMembersQuery : IRequest<Response<List<HouseMemberDto>>>
    {
        public int HouseId { get; set; }
    }
}
